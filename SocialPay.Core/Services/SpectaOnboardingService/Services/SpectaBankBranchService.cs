using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.SpectaOnboarding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaBankBranchService : ISpectaBankBranch
    {

        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;

        public SpectaBankBranchService(ISpectaOnBoarding spectaOnboardingService, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _spectaOnboardingService = spectaOnboardingService;
            _spectaOnboardingLogger = spectaOnboardingLogger;
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
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- BankBranchList " + ex.ToString()}{"-"}{DateTime.Now}", true);

                return response;

            }
        }
    }
}
