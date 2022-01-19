using SocialPay.Core.Services.SpectaOnboardingService.Interface;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaLoggedInCustomerProfile : ISpectaLoggedInCustomerProfile
    {
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaLoggedInCustomerProfile));

        public SpectaLoggedInCustomerProfile(ISpectaOnBoarding spectaOnboardingService)
        {
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> LoggedInCustomerProfile(string email)
        {

            try
            {
                return await _spectaOnboardingService.LoggedInCustomerProfile(email);

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "LoggedIn Customer Profile" + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse
                {
                    ResponseCode = AppResponseCodes.InternalError, Data = "Internal error occured"
                };
            }
        }
    }

}
