using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaBankBranchService : ISpectaBankBranch
    {

        private readonly ISpectaOnBoarding _spectaOnboardingService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaBankBranchService));

        public SpectaBankBranchService(ISpectaOnBoarding spectaOnboardingService)
        {
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> BankBranchList()
        {
            var response = await _spectaOnboardingService.BankBranchList();

            try
            {
                return response;

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "BankBranchList" + " | " + ex + " | " + DateTime.Now);

                return response;

            }
        }
    }
}
