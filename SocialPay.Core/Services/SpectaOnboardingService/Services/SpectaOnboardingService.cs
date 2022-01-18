using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.SpectaOnboardingService.Interface;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaOnboardingService : ISpectaOnBoarding
    {
        private readonly AppSettings _appSettings;
        private readonly SpectaOnboardingSettings _spectaOnboardingSettings;
        private readonly IAuthentication _authentication;
        private readonly HttpClient _client;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaOnboardingService));

        public SpectaOnboardingService(IOptions<AppSettings> appSettings, IOptions<SpectaOnboardingSettings> spectaOnboardingSettings, IAuthentication authentication)
        {
            _appSettings = appSettings.Value;
            _spectaOnboardingSettings = spectaOnboardingSettings.Value;
            _authentication = authentication;
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
                //request.AddHeader("Abp.TenantId", _appSettings.SpectaRegistrationTenantId);
                //request.AddHeader("Authorization", "Bearer Bearer " + await _authentication.AccessTokenTesting(model.emailAddress));
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
                _log4net.Error("Error occured" + " | " + "RegisterCustomerInfo" + " | " + model.bvn + " | " + model.emailAddress + " | " + model.name + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> SendEmailVerificationCode(SendEmailVerificationCodeRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.SendEmailVerificationCodeUrlExtension}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                //request.AddHeader("Abp.TenantId", _appSettings.SpectaRegistrationTenantId);
                //request.AddHeader("Authorization", "Bearer Bearer " + await _authentication.AccessTokenTesting(model.email));
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));

                apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;

                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<SpectaResponseWithBoolResultMessage.SpectaResponseDto>(response.Content);
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
                _log4net.Error("Error occured" + " | " + "SendEmailVerificationCodeInfo" + " | " + model.email + " | " + model.clientBaseUrl + " | " + model.verificationCodeParameterName + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
            }

         }

        public async Task<WebApiResponse> VerifyEmailConfirmationCode(VerifyEmailConfirmationCodeRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.VerifyEmailConfirmationCodeUrlExtension}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
               // request.AddHeader("Abp.TenantId", _appSettings.SpectaRegistrationTenantId);
                //request.AddHeader("Authorization", "Bearer Token " + await _authentication.AccessTokenTesting(model.email));
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
                _log4net.Error("Error occured" + " | " + "VerifyEmailConfirmationCodeInfo" + " | " + model.email + " | " + model.token + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> SendBvnPhoneVerificationCode(string emailaddress)
        {
            var apiResponse = new WebApiResponse { };
            try
            {

                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.SendBvnPhoneVerificationCodeUrlExtension}{emailaddress}");
                client.Timeout = -1;
                
                var request = new RestRequest(Method.POST);
               // request.AddHeader("Authorization", "Bearer Bearer " + await _authentication.AccessTokenTesting(emailaddress));
                request.AlwaysMultipartFormData = true;
                request.AddParameter("emailAddress", emailaddress);
                
                IRestResponse response = await Task.FromResult(client.Execute(request));
                ////var Response = JsonConvert.DeserializeObject<SpectaResponseWithObjectResultMessage.SpectaResponseDto>(response.Content);
                ////apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;
                ////apiResponse.Data = Response;

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
                _log4net.Error("Error occured" + " | " + "SendBvnPhoneVerificationCodeInfo" + " | " + emailaddress + " | " + ex + " | " + DateTime.Now);

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
                //request.AddHeader("Abp.TenantId", _appSettings.TenantId);
               // request.AddHeader("Authorization", "Bearer Bearer " + await _authentication.AccessTokenTesting(model.email));
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));

                ////var Response = JsonConvert.DeserializeObject<SpectaResponseWithObjectResultMessage.SpectaResponseDto>(response.Content);
                ////apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;
                ////apiResponse.Data = Response;
                ////return apiResponse;

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
                _log4net.Error("Error occured" + " | " + "VerifyBvnPhoneConfirmationInfo" + " | " + model.email + " | " + model.token + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
        public async Task<WebApiResponse> LoggedInCustomerProfile(string email)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                //var requestobj = JsonConvert.SerializeObject(model);

                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.LoggedInCustomerProfileUrlExtension}");

                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = await Task.FromResult(client.Execute(request));
             
                apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;

                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<LoggedInCustomerProfileResponseDto.LoggedInCustomerProfileResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.StatusCode = ResponseCodes.Success;
                    apiResponse.Message = "Success";

                    return apiResponse;
                }

                apiResponse.Data = response.Content;
                apiResponse.StatusCode = ResponseCodes.InternalError;

                return apiResponse;

            }
            catch (Exception)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
        public async Task<WebApiResponse> AddOrrInformation(AddOrrInformationRequestDto model, string email)
        {
            var apiResponse = new WebApiResponse { };

            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.AddOrrInformationUrlExtension}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
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
                _log4net.Error("Error occured" + " | " + "AddOrrInformation" + " | " + model.incomeSource + " | " + model.jobChanges + " | " + model.natureOfIncome + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }

        public async Task<WebApiResponse> Authenticate(AuthenticateRequestDto model)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.AuthenticaUrlExtensionUrl}");

               // var client = new RestClient(_client.BaseAddress + _appSettings.AuthenticaUrlExtension);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
               // request.AddHeader("Abp.TenantId", _appSettings.TenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));

                //var Response = JsonConvert.DeserializeObject<AuthenticateResponseDto.AuthenticateResponse>(response.Content);
                //apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;
                //apiResponse.Data = Response.result;
                //apiResponse.StatusCode = ResponseCodes.Success;



                apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;

                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<AuthenticateResponseDto.AuthenticateResponse>(response.Content);
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
                _log4net.Error("Error occured" + " | " + "AuthenticateInfo" + " | " + model.password + " | " + model.userNameOrEmailAddress + " | " + model.rememberClient + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return apiResponse;
            }

        }

        public async Task<WebApiResponse> BusinessSegmentAllList(string email)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var client = new RestClient($"{_client.BaseAddress}{ _spectaOnboardingSettings.BusinessSegmentAllListUrlExtension}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = await Task.FromResult(client.Execute(request));

                apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;

                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<BusinessSegmentAllListResponseDto.BusinessSegmentAllListResponse>(response.Content);
                    apiResponse.Data = Response;
                    apiResponse.StatusCode = ResponseCodes.Success;
                    apiResponse.Message = "Success";

                    return apiResponse;
                }

                apiResponse.Data = response.Content;
                apiResponse.StatusCode = ResponseCodes.InternalError;

                return apiResponse;

            }
            catch (Exception)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Internal error occured. Please try again", Data = "Internal error occured. Please try again", StatusCode = ResponseCodes.InternalError };
            }

        }

        public async Task<WebApiResponse> RequestTicket(RequestTicketDto model, string email)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.RequestTicketUrlExtension}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));               

                apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;

                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<RequestTicketResponseDto.RequestTicketResponse>(response.Content);
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
                _log4net.Error("Error occured" + " | " + "RequestTicketInfo" + " | " + model.accountNumber + " | " + model.bankId + " | " + model.customerId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

          }
        public async Task<WebApiResponse> ConfirmTicket(ConfirmTicketRequestDto model, string email)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.ConfirmTicketUrlExtension}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));              

                apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;

                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<ConfirmTicketResponseDto.ConfirmTicketResponse>(response.Content);
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
                _log4net.Error("Error occured" + " | " + "ConfirmTicketInfo" + " | " + model.accountNumber + " | " + model.bankId + " | " + model.customerId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> CreateIndividualCurrentAccount(CreateIndividualCurrentAccountRequestDto model, string email)
        {
            var apiResponse = new WebApiResponse { };
            try
            {

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
                    client.DefaultRequestHeaders.Add("Abp.TenantId", "1");
                    HttpResponseMessage result;
                    using (var formContent = new MultipartFormDataContent())
                    {

                        formContent.Add(new StringContent(model.BranchCode), "BranchCode");
                        formContent.Add(new StringContent(model.TaxId), "TaxId");
                        formContent.Add(new StringContent(model.CountryOfBirth), "CountryOfBirth");
                        formContent.Add(new StringContent(model.OtherNationality), "OtherNationality");

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

                        result = await client.PostAsync($"{_client.BaseAddress}{_spectaOnboardingSettings.CreateIndividualCurrentAccountUrlExtension}", formContent);
                    }

                    var res1 = await result.Content.ReadAsStringAsync();

                    apiResponse.ResponseCode = result.IsSuccessStatusCode == true ? AppResponseCodes.Success : AppResponseCodes.Failed;

                    if (result.IsSuccessStatusCode)
                    {
                        var Response = JsonConvert.DeserializeObject<CreateIndividualCurrentAccountResponseDto.CreateIndividualCurrentAccountResponse>(res1);
                        apiResponse.Data = Response;
                        apiResponse.StatusCode = ResponseCodes.Success;
                        apiResponse.Message = "Success";

                        return apiResponse;
                    }

                    apiResponse.Data = res1;
                    apiResponse.StatusCode = ResponseCodes.InternalError;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "CreateIndividualCurrentAccountInfo" + " | " + model.BranchCode + " | " + model.CountryOfBirth + " | " + model.Passport + " | " + model.Signature + " | " + model.TaxId + " | " + model.UtilityBill + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

          }
        public async Task<WebApiResponse> DisbursementAccount(SetDisbursementAccountRequestDto model, string email)
        {
            var apiResponse = new WebApiResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.DisbursementAccountUrlExtension}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));             

                apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;

                if (response.IsSuccessful)
                {
                    var Response = JsonConvert.DeserializeObject<SpectaResponseWithObjectResultMessage.SpectaResponseDto>(response.Content); apiResponse.Data = Response;
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
                _log4net.Error("Error occured" + " | " + "DisbursementAccountInfo" + " | " + model.disbAccountNumber + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

           }
            ////public async Task<WebApiResponse> ChargeCard(ChargeCardRequestDto model, string email)
            ////{
            ////    var apiResponse = new WebApiResponse { };
            ////    try
            ////    {
            ////        var requestobj = JsonConvert.SerializeObject(model);
            ////        var client = new RestClient(_client.BaseAddress + _appSettings.ChargeCardUrlExtension);
            ////        client.Timeout = -1;
            ////        var request = new RestRequest(Method.POST);
            ////        request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
            ////        request.AddHeader("Abp.TenantId", _appSettings.TenantId);
            ////        request.AddHeader("Content-Type", "application/json");
            ////        request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
            ////        IRestResponse response = await Task.FromResult(client.Execute(request));
            ////        var Response = JsonConvert.DeserializeObject<PaystackTokennizationResponseDto.PaystackTokennizationResponse>(response.Content);
            ////        Response.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;
            ////        apiResponse.Data = Response;
            ////        return apiResponse;
            ////    }
            ////    catch (Exception ex)
            ////    {
            ////        _log4net.Error("Error occured" + " | " + "ChargeCardInfo" + " | " + model.cvv + " | " + " | " + model.expiryMonth + " | " + " | " + model.expiryYear + " | " + " | " + model.pin + " | " + ex.Message.ToString() + " | " + DateTime.Now);
            ////        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            ////    }

            ////}
            ////public async Task<WebApiResponse> SendPhone(SendPhoneRequestDto model, string email)
            ////{
            ////    var apiResponse = new WebApiResponse { };
            ////    try
            ////    {
            ////        var requestobj = JsonConvert.SerializeObject(model);
            ////        var client = new RestClient(_client.BaseAddress + _appSettings.SendPhoneUrlExtension);
            ////        client.Timeout = -1;
            ////        var request = new RestRequest(Method.POST);
            ////        request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
            ////        request.AddHeader("Abp.TenantId", _appSettings.TenantId);
            ////        request.AddHeader("Content-Type", "application/json");
            ////        request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
            ////        IRestResponse response = await Task.FromResult(client.Execute(request));
            ////        var Response = JsonConvert.DeserializeObject<PaystackTokennizationResponseDto.PaystackTokennizationResponse>(response.Content);
            ////        apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;
            ////        apiResponse.Data = Response;
            ////        return apiResponse;
            ////    }
            ////    catch (Exception ex)
            ////    {
            ////        _log4net.Error("Error occured" + " | " + "SendPhoneInfo" + " | " + model.cardId + " | " + " | " + model.phoneNumber + " | " + ex.Message.ToString() + " | " + DateTime.Now);

            ////        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            ////    }

            ////}
            ////public async Task<WebApiResponse> SendOtp(SendOtpRequestDto model, string email)
            ////{
            ////    var apiResponse = new WebApiResponse { };

            ////    try
            ////    {
            ////        var requestobj = JsonConvert.SerializeObject(model);
            ////        var client = new RestClient(_client.BaseAddress + _appSettings.SendOtpUrlExtension);
            ////        client.Timeout = -1;
            ////        var request = new RestRequest(Method.POST);
            ////        request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
            ////        request.AddHeader("Abp.TenantId", _appSettings.TenantId);
            ////        request.AddHeader("Content-Type", "application/json");
            ////        request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
            ////        IRestResponse response = await Task.FromResult(client.Execute(request));
            ////        var Response = JsonConvert.DeserializeObject<PaystackTokennizationResponseDto.PaystackTokennizationResponse>(response.Content);
            ////        apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;
            ////        apiResponse.Data = Response;
            ////        return apiResponse;
            ////    }
            ////    catch (Exception ex)
            ////    {
            ////        _log4net.Error("Error occured" + " | " + "SendOtpInfo" + " | " + model.cardId + " | " + model.otp + " | " + ex.Message.ToString() + " | " + DateTime.Now);
            ////        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            ////    }

            ////}
            ////public async Task<WebApiResponse> SendPin(SendPinRequestDto model, string email)
            ////{
            ////    var apiResponse = new WebApiResponse { };
            ////    try
            ////    {
            ////        var requestobj = JsonConvert.SerializeObject(model);
            ////        var client = new RestClient(_client.BaseAddress + _appSettings.SendPinUrlExtension);
            ////        client.Timeout = -1;
            ////        var request = new RestRequest(Method.POST);
            ////        request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
            ////        request.AddHeader("Abp.TenantId", _appSettings.TenantId);
            ////        request.AddHeader("Content-Type", "application/json");
            ////        request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
            ////        IRestResponse response = await Task.FromResult(client.Execute(request));
            ////        var Response = JsonConvert.DeserializeObject<PaystackTokennizationResponseDto.PaystackTokennizationResponse>(response.Content);
            ////        apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;
            ////        apiResponse.Data = Response;
            ////        return apiResponse;
            ////    }
            ////    catch (Exception ex)
            ////    {
            ////        _log4net.Error("Error occured" + " | " + "SendPinInfo" + " | " + model.cardId + " | " + model.pin + " | " + ex.Message.ToString() + " | " + DateTime.Now);
            ////        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            ////    }

            ////}
            ////public async Task<WebApiResponse> ValidateCharge(ValidateChargeRequestDto model, string email)
            ////{
            ////    var apiResponse = new WebApiResponse { };
            ////    try
            ////    {
            ////        var requestobj = JsonConvert.SerializeObject(model);
            ////        var client = new RestClient(_client.BaseAddress + _appSettings.ValidateChargeUrlExtension);
            ////        client.Timeout = -1;
            ////        var request = new RestRequest(Method.POST);
            ////        request.AddHeader("Authorization", "Bearer " + await _authentication.AccessTokenTesting(email));
            ////        request.AddHeader("Abp.TenantId", _appSettings.TenantId);
            ////        request.AddHeader("Content-Type", "application/json");
            ////        request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
            ////        IRestResponse response = await Task.FromResult(client.Execute(request));
            ////        var Response = JsonConvert.DeserializeObject<PaystackTokennizationResponseDto.PaystackTokennizationResponse>(response.Content);
            ////        apiResponse.ResponseCode = response.IsSuccessful == true ? AppResponseCodes.Success : AppResponseCodes.Failed;
            ////        apiResponse.Data = Response;
            ////        return apiResponse;
            ////    }
            ////    catch (Exception ex)
            ////    {
            ////        _log4net.Error("Error occured" + " | " + "ValidateChargeInfo" + " | " + model.cardId + " | " + ex.Message.ToString() + " | " + DateTime.Now);
            ////        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            ////    }

            ////}

        }

}
