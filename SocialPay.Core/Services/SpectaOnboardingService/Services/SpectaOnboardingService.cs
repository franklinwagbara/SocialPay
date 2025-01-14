﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.SpectaOnboarding;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace SocialPay.Core.Services.Specta
{
    public class SpectaOnboardingService : ISpectaOnBoarding
    {
        private readonly AppSettings _appSettings;
        private readonly SpectaOnboardingSettings _spectaOnboardingSettings;
        private readonly IAuthentication _authentication;
        private readonly HttpClient _client;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;
        public SpectaOnboardingService(IOptions<AppSettings> appSettings, IOptions<SpectaOnboardingSettings> spectaOnboardingSettings, IAuthentication authentication, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _appSettings = appSettings.Value;
            _spectaOnboardingSettings = spectaOnboardingSettings.Value;
            _authentication = authentication;
            _spectaOnboardingLogger = spectaOnboardingLogger;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.paywithSpectaBaseUrl),
            };
        }

        public async Task<WebApiResponse> RegisterCustomer(RegisterCustomerRequestDto model)
        {
            var apiResponse = new WebApiResponse { };

            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.SpectaRegistrationCustomerUrlExtension}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));

                apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;

                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<SpectaResponseWithObjectResultMessage.SpectaResponseDto>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.StatusCode = ResponseCodes.Success;
                    apiResponse.Message = "Success";

                    return apiResponse;
                }

                apiResponse.Data = response.Content;
                apiResponse.StatusCode = ResponseCodes.InternalError;
                return apiResponse;

            }
            catch (Exception ex)
            {
             
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- RegisterCustomerInfo "+ ex.ToString()}{"-"}{model.bvn}{"-"}{model.emailAddress}{"-"}{model.name}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }


        public async Task<WebApiResponse> SendEmailVerificationCode(SendEmailVerificationCodeRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.SendEmailVerificationCodeUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                //request.AddHeader("Abp.TenantId", _appSettings.TenantId);
                //request.AddHeader("Authorization", "Bearer Bearer " + await _authentication.AccessTokenTesting(model.email));
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<SpectaResponseWithBoolResultMessage.SpectaResponseDto>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- SendEmailVerificationCodeInfo " + ex.ToString()}{"-"}{model.email}{"-"}{model.clientBaseUrl}{"-"}{model.verificationCodeParameterName}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }


        }

        public async Task<WebApiResponse> VerifyEmailConfirmationCode(VerifyEmailConfirmationCodeRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.VerifyEmailConfirmationCodeUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<SpectaResponseWithObjectResultMessage.SpectaResponseDto>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                if (response.Content.Contains("OTP Expired"))
                {
                    apiResponse.Data = response.Content;
                    apiResponse.ResponseCode = AppResponseCodes.OtpExpired;
                    apiResponse.Message = "OTP Expired";
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- VerifyEmailConfirmationCodeInfo " + ex.ToString()}{"-"}{model.email}{"-"}{model.token}{"-"}{DateTime.Now}", true);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> SendBvnPhoneVerificationCode(string emailaddress)
        {
            var apiResponse = new WebApiResponse { };
            try
            {

                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.SendBvnPhoneVerificationCodeUrlExtension + emailaddress);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                request.AddParameter("emailAddress", emailaddress);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<SpectaResponseWithObjectResultMessage.SpectaResponseDto>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                if (response.Content.Contains("OTP Expired"))
                {
                    apiResponse.Data = response.Content;
                    apiResponse.ResponseCode = AppResponseCodes.OtpExpired;
                    apiResponse.Message = "OTP Expired";
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- SendBvnPhoneVerificationCodeInfo " + ex.ToString()}{"-"}{emailaddress}{"-"}{"-"}{DateTime.Now}", true);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
        public async Task<WebApiResponse> VerifyBvnPhoneConfirmationCode(VerifyBvnPhoneConfirmationCodeRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.VerifyBvnPhoneConfirmationCodeUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));

                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<SpectaResponseWithObjectResultMessage.SpectaResponseDto>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                if (response.Content.Contains("OTP Expired"))
                {
                    apiResponse.Data = response.Content;
                    apiResponse.ResponseCode = AppResponseCodes.OtpExpired;
                    apiResponse.Message = "OTP Expired";
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- VerifyBvnPhoneConfirmationInfo " + ex.ToString()}{"-"}{model.email}{"-"}{model.token}{"-"}{DateTime.Now}", true);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
        public async Task<WebApiResponse> LoggedInCustomerProfile(string email)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.LoggedInCustomerProfileUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<LoggedInCustomerProfileResponseDto.LoggedInCustomerProfileResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
        public async Task<WebApiResponse> AddOrrInformation(AddOrrInformationRequestDto model)
        {
            var apiResponse = new WebApiResponse { };

            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.AddOrrInformationUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(model.Email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");

                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<SpectaResponseWithObjectResultMessage.SpectaResponseDto>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- AddOrrInformation " + ex.ToString()}{"-"}{model.incomeSource}{"-"}{model.jobChanges}{"-"}{model.natureOfIncome}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }


        public async Task<WebApiResponse> Authenticate(AuthenticateRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.AuthenticaUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<AuthenticateResponseDto.AuthenticateResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- AuthenticateInfo " + ex.ToString()}{"-"}{model.password}{"-"}{model.userNameOrEmailAddress}{"-"}{model.rememberClient}{"-"}{DateTime.Now}", true);

                return apiResponse;
            }

        }



        public async Task<WebApiResponse> BusinessSegmentAllList()
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.BusinessSegmentAllListUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<BusinessSegmentAllListResponseDto.BusinessSegmentAllListResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }

        public async Task<WebApiResponse> RequestTicket(RequestTicketDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.RequestTicketUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(model.Email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<RequestTicketResponseDto.RequestTicketResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- RequestTicketInfo " + ex.ToString()}{"-"}{model.accountNumber}{"-"}{model.bankId}{"-"}{model.customerId}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> ConfirmTicket(ConfirmTicketRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.ConfirmTicketUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(model.Email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<ConfirmTicketResponseDto.ConfirmTicketResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- ConfirmTicketInfo " + ex.ToString()}{"-"}{model.accountNumber}{"-"}{model.bankId}{"-"}{model.customerId}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> CreateIndividualCurrentAccount(CreateIndividualCurrentAccountRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {


                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await _authentication.AccessTokenTesting(model.Email));
                    client.DefaultRequestHeaders.Add("Abp.TenantId", "1");
                    HttpResponseMessage result;
                    using (var formContent = new MultipartFormDataContent())
                    {

                        formContent.Add(new StringContent(model.BranchCode), "BranchCode");
                        formContent.Add(new StringContent(model.TaxId), "TaxId");
                        formContent.Add(new StringContent(model.CountryOfBirth), "CountryOfBirth");
                        formContent.Add(new StringContent(model.OtherNationality != null ? model.OtherNationality : ""), "OtherNationality");

                        using (var ms = new MemoryStream())
                        {
                            model.IdentityCard.CopyTo(ms);
                            var data = new ByteArrayContent(ms.ToArray());
                            data.Headers.ContentType = new MediaTypeHeaderValue(model.IdentityCard.ContentType);
                            formContent.Add(data, nameof(model.IdentityCard), nameof(model.IdentityCard));
                        }
                        using (var ms = new MemoryStream())
                        {
                            model.Passport.CopyTo(ms);
                            var data = new ByteArrayContent(ms.ToArray());
                            data.Headers.ContentType = new MediaTypeHeaderValue(model.Passport.ContentType);
                            formContent.Add(data, nameof(model.Passport), nameof(model.Passport));
                        }
                        using (var ms = new MemoryStream())
                        {
                            model.Signature.CopyTo(ms);
                            var data = new ByteArrayContent(ms.ToArray());
                            data.Headers.ContentType = new MediaTypeHeaderValue(model.Signature.ContentType);
                            formContent.Add(data, nameof(model.Signature), nameof(model.Signature));
                        }
                        using (var ms = new MemoryStream())
                        {
                            model.UtilityBill.CopyTo(ms);
                            var data = new ByteArrayContent(ms.ToArray());
                            data.Headers.ContentType = new MediaTypeHeaderValue(model.UtilityBill.ContentType);
                            formContent.Add(data, nameof(model.UtilityBill), nameof(model.UtilityBill));
                        }

                        result = await client.PostAsync(_client.BaseAddress + _spectaOnboardingSettings.CreateIndividualCurrentAccountUrlExtension, formContent);
                    }

                    var res1 = await result.Content.ReadAsStringAsync();
                    var Response = JsonConvert.DeserializeObject<CreateIndividualCurrentAccountResponseDto.CreateIndividualCurrentAccountResponse>(res1);
                    apiResponse.ResponseCode = Response.success == true ? AppResponseCodes.Success : AppResponseCodes.Failed;
                    apiResponse.Data = Response;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- CreateIndividualCurrentAccountInfo " + ex.ToString()}{"-"}{model.BranchCode}{"-"}{model.CountryOfBirth}{"-"}{model.Passport}{"-"}{model.Signature}{"-"}{model.TaxId}{"-"}{model.UtilityBill}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> DisbursementAccount(SetDisbursementAccountRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.DisbursementAccountUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(model.Email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<SpectaResponseWithObjectResultMessage.SpectaResponseDto>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- DisbursementAccountInfo " + ex.ToString()}{"-"}{model.disbAccountNumber}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> ChargeCard(ChargeCardRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.ChargeCardUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(model.Email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<PaystackTokennizationResponseDto.PaystackTokennizationResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- ChargeCardInfo " + ex.ToString()}{"-"}{model.cvv}{"-"}{model.expiryMonth}{"-"}{model.expiryYear}{"-"}{model.pin}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> SendPhone(SendPhoneRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.SendPhoneUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(model.Email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<PaystackTokennizationResponseDto.PaystackTokennizationResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- SendPhoneInfo " + ex.ToString()}{"-"}{model.cardId}{"-"}{model.phoneNumber}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> SendOtp(SendOtpRequestDto model)
        {
            var apiResponse = new WebApiResponse { };

            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.SendOtpUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(model.Email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<PaystackTokennizationResponseDto.PaystackTokennizationResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    apiResponse.StatusCode = ResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- SendPhoneInfo " + ex.ToString()}{"-"}{model.cardId}{"-"}{model.otp}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> SendPin(SendPinRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.SendPinUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(model.Email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<PaystackTokennizationResponseDto.PaystackTokennizationResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    apiResponse.StatusCode = ResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- SendPinInfo " + ex.ToString()}{"-"}{model.cardId}{"-"}{model.pin}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> ValidateCharge(ValidateChargeRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.ValidateChargeUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(model.Email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<PaystackTokennizationResponseDto.PaystackTokennizationResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    apiResponse.StatusCode = ResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                return apiResponse;
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- ValidateChargeInfo " + ex.ToString()}{"-"}{model.cardId}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }

        public async Task<WebApiResponse> BankBranchList()
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.BankBranchListUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<BankBranchDto.BankBranchDtoResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    apiResponse.StatusCode = ResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                apiResponse.StatusCode = ResponseCodes.Success;
                return apiResponse;

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ex.ToString() };
            }
        }

        public async Task<WebApiResponse> AvailableBanksList(string email)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var client = new RestClient(_client.BaseAddress + _spectaOnboardingSettings.AvailableBankListUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = await Task.FromResult(client.Execute(request));
                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<AvailableBanksDto.AvailableBanksDtoResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    apiResponse.StatusCode = ResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.Data = response.Content;
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                apiResponse.StatusCode = ResponseCodes.Success;
                return apiResponse;

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ex.ToString(), StatusCode = ResponseCodes.InternalError };
            }
        }


    }
}
