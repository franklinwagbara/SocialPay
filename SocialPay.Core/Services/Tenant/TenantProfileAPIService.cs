using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Core.Configurations;
using SocialPay.Core.Messaging;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Tenant
{
    public class TenantProfileAPIService
    {
        private readonly ITenantProfileService _tenantProfileService;
        private readonly HttpClient _client;
        private readonly EmailService _emailService;
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(TenantProfileAPIService));
        public TenantProfileAPIService(ITenantProfileService tenantProfileService,
             EmailService emailService,
            IOptions<AppSettings> appSettings)
        {
            _tenantProfileService = tenantProfileService ?? throw new ArgumentNullException(nameof(tenantProfileService));
            _emailService = emailService;
            _appSettings = appSettings.Value;

            _client = new HttpClient
            {
                //BaseAddress = new Uri(_appSettings.BaseUrlTenant),
                BaseAddress = new Uri("http://localhost:53237"),
                
            };
        }

        public async Task<WebApiResponse> CreateNewTenant(TenantProfileRequestDto request, string email)
        {
            try
            {
                _log4net.Info("CreateNewTenant " + DateTime.Now);
                request.UserId = email;
                var jsonRequest = JsonConvert.SerializeObject(request);

                _client.DefaultRequestHeaders.Add("ClientId", "0");
                _client.DefaultRequestHeaders.Add("ClientSecret", "0");
                _client.DefaultRequestHeaders.Add("AuthKey", "0");

                var req = await _client.PostAsync(_appSettings.createTenantUrl,
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var content = await req.Content.ReadAsStringAsync();
                var successfulResponse = JsonConvert.DeserializeObject<TenantResponseDTO>(content);

                if (successfulResponse.data.responseCode == "00")
                {

                    var emailModal = new EmailRequestDto
                    {
                        Subject = "Tenant Sign Up",
                        DestinationEmail = request.Email.ToLower(),

                    };

                    //
                    var mailBuilder = new StringBuilder();
                    mailBuilder.AppendLine("Dear" + " " + request.Email.ToLower() + "," + "<br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("You have successfully created a ternant <br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("Client Id -" + successfulResponse.data.data.clientId);
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("Client Secret -" + successfulResponse.data.data.clientSecret);
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("AuthKey -" + successfulResponse.data.data.authKey);
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("Best Regards,");

                    emailModal.EmailBody = mailBuilder.ToString();

                    var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                    if (sendMail != AppResponseCodes.Success)
                        return new WebApiResponse { ResponseCode = successfulResponse.data.responseCode, StatusCode = ResponseCodes.InternalError, Message = "Ternant created but email service failed", Data = successfulResponse.data.data };

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = "Success" ,StatusCode = ResponseCodes.Success};
                }

                return new WebApiResponse { ResponseCode = successfulResponse.data.responseCode, Message = successfulResponse.data.message, Data = successfulResponse.data.data };

            }
            catch (Exception ex)
            {
                _log4net.Error("CreateNewTenant error occured " + " - "+ ex + " - "+  DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Internal error occured", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetTenant(long clientId)
        {
            try
            {
                _client.DefaultRequestHeaders.Add("ClientId", "0");
                _client.DefaultRequestHeaders.Add("ClientSecret", "0");
                _client.DefaultRequestHeaders.Add("AuthKey", "0");
                _client.DefaultRequestHeaders.Add("userId", "0");

                var response = await _client.GetAsync($"{_appSettings.getTenantUrl}");

                if (!response.IsSuccessStatusCode)
                    new WebApiResponse { ResponseCode = AppResponseCodes.Failed,StatusCode = ResponseCodes.InternalError };

                var content = await response.Content.ReadAsStringAsync();
                List< GetTenantResponseDto> successfulResponse = JsonConvert.DeserializeObject<List<GetTenantResponseDto>>(content);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = successfulResponse, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("GetTenant error occured " + " - " + ex + " - " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }
        //public async Task<WebApiResponse> CreateNewTenant(TenantProfileRequestDto request, long clientId)
        //{
        //    try
        //    {
        //        if (await _tenantProfileService.ExistsByEmailAsync(request.Email.ToLower(), request.PhoneNumber))
        //            return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails, Message = "Duplicate email or phone number" };

        //        var model = new TenantProfileViewModel
        //        {
        //            Address = request.Address,
        //            Email = request.Email.ToLower(),
        //            PhoneNumber = request.PhoneNumber,
        //            TenantName = request.TenantName,
        //            WebSiteUrl = request.WebSiteUrl,
        //            ClientAuthenticationId = clientId
        //        };

        //        await _tenantProfileService.AddAsync(model);

        //        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Tenant was successfully created" };

        //    }
        //    catch (Exception ex)
        //    {
        //        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Internal error occured" };
        //    }
        //}


        //public async Task<WebApiResponse> GetTenant(long clientId)
        //{
        //    try
        //    {
        //        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = await _tenantProfileService.GetAllAsync() };
        //    }
        //    catch (Exception ex)
        //    {

        //        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured" };
        //    }
        //}
    }
}
