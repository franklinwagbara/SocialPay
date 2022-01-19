using SocialPay.Core.Services.SpectaOnboardingService.Interface;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaAvailableBanksService : ISpectaAvailableBanks
    {
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaAvailableBanksService));

        public SpectaAvailableBanksService(ISpectaOnBoarding spectaOnboardingService)
        {
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> AvailableBanksList(string email)
        {
            var response = await _spectaOnboardingService.AvailableBanksList(email);

            try
            {
                return response;

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "AvailableBanksList" + " | " + ex + " | " + DateTime.Now);

                return response;

            }


        }
    }
}
