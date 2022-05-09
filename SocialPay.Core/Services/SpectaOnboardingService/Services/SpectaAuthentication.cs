using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.SpectaOnboarding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaAuthentication : ISpectaAuthentication
    {
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;
        private readonly ISpectaOnBoarding _spectaOnboardingService;

        public SpectaAuthentication(ISpectaOnBoarding spectaOnboardingService, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _spectaOnboardingService = spectaOnboardingService;
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }

        public async Task<WebApiResponse> Authenticate(AuthenticateRequestDto model)
        {

            var response = await _spectaOnboardingService.Authenticate(model);

            try
            {
                return new WebApiResponse { ResponseCode = response.ResponseCode, Message = "Success", Data = response.Data, StatusCode = ResponseCodes.Success };

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- Authentication "+ex.ToString()}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = response.ResponseCode, Message = "Request Failed", Data = response.Data };

            }
        }

    }

}
