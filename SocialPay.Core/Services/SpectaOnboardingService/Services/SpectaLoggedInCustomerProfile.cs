using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.SpectaOnboarding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaLoggedInCustomerProfile : ISpectaLoggedInCustomerProfile
    {
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;

        public SpectaLoggedInCustomerProfile(ISpectaOnBoarding spectaOnboardingService, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _spectaOnboardingService = spectaOnboardingService;
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }

        public async Task<WebApiResponse> LoggedInCustomerProfile(string email)
        {

            try
            {
                return await _spectaOnboardingService.LoggedInCustomerProfile(email);

            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- LoggedIn Customer Profile " + ex.ToString()}{"-"}{DateTime.Now}", true);

                return new WebApiResponse
                {
                    ResponseCode = AppResponseCodes.InternalError, Data = "Internal error occured"
                };
            }
        }
    }

}
