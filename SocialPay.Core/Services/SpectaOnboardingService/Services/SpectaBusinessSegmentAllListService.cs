using SocialPay.Core.Services.SpectaOnboardingService.Interface;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaBusinessSegmentAllListService : ISpectaBusinessSegmentAllList
    {
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaBusinessSegmentAllListService));

        public SpectaBusinessSegmentAllListService(ISpectaOnBoarding spectaOnboardingService)
        {
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> BusinessSegmentAllList(string email)
        {
            return await _spectaOnboardingService.BusinessSegmentAllList(email);
        }
    }
}
