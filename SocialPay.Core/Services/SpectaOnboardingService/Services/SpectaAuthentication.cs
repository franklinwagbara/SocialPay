using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaAuthentication : ISpectaAuthentication
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaAuthentication));
        private readonly ISpectaOnBoarding _spectaOnboardingService;

        public SpectaAuthentication(ISpectaOnBoarding spectaOnboardingService)
        {
            _spectaOnboardingService = spectaOnboardingService;
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
                _log4net.Error("Error occured" + " | " + "Authentication" + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = response.ResponseCode, Message = "Request Failed", Data = response.Data };

            }
        }

    }

}
