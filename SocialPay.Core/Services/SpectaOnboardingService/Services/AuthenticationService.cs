using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Domain;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.SpectaOnboarding;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class AuthenticationService : IAuthentication
    {
        private readonly AppSettings _appSettings;
        private readonly SpectaOnboardingSettings _spectaOnboardingSettings;
        private readonly SocialPayDbContext _context;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;
        private readonly HttpClient _client;
        public AuthenticationService(IOptions<AppSettings> appSettings, IOptions<SpectaOnboardingSettings> spectaOnboardingSettings, SocialPayDbContext context, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _appSettings = appSettings.Value;
            _spectaOnboardingSettings = spectaOnboardingSettings.Value;
            _context = context;
            _spectaOnboardingLogger = spectaOnboardingLogger;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.paywithSpectaBaseUrl),
            };
        }

        public async Task<AuthenticateResponseDto.AuthenticateResponse> AccessTokenAuthentication(AuthenticateRequestDto model)
        {
            var apiResponse = new AuthenticateResponseDto.AuthenticateResponse { };
            try
            {
                var requestobj = JsonConvert.SerializeObject(model);
                var client = new RestClient($"{_client.BaseAddress}{_spectaOnboardingSettings.SpectaRegistrationAuthenticaUrlExtension}");
                client.Timeout = -1;

                var request = new RestRequest(Method.POST);
                request.AddHeader("Abp.TenantId", _spectaOnboardingSettings.SpectaRegistrationTenantId);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestobj, ParameterType.RequestBody);
                IRestResponse response = await Task.FromResult(client.Execute(request));

                apiResponse = JsonConvert.DeserializeObject<AuthenticateResponseDto.AuthenticateResponse>(response.Content);
                return apiResponse;
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- AuthenticateInfo" }{ex}{"-"}{model.password}{"-"}{model.userNameOrEmailAddress}{"-"}{model.rememberClient}", true);
                return apiResponse;
            }

        }

        public async Task<string> AccessTokenTesting(string emailaddress)
        {
            var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == emailaddress);
            if (checkregistered != null)
            {
                var password = checkregistered.password;
                var email = checkregistered.emailAddress;
                var model = new AuthenticateRequestDto();
                if (checkregistered != null)
                {

                    model.password = password.Decrypt(_appSettings.appKey);
                    model.userNameOrEmailAddress = email;
                    model.rememberClient = true;
                }
                AuthenticateResponseDto.AuthenticateResponse Response = await AccessTokenAuthentication(model);
                return Response.result.accessToken;
            }

            return "";

        }

    }

}
