using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.SpectaOnboarding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaBusinessSegmentAllListService : ISpectaBusinessSegmentAllList
    {
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;

        public SpectaBusinessSegmentAllListService(ISpectaOnBoarding spectaOnboardingService, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _spectaOnboardingService = spectaOnboardingService;
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }

        public async Task<WebApiResponse> BusinessSegmentAllList()
        {
            _spectaOnboardingLogger.LogRequest($"{"BusinessSegmentAllList -- About to get All bussiness segment list"}{"-"}{DateTime.Now}", true);
            return await _spectaOnboardingService.BusinessSegmentAllList();
        }
    }
}
