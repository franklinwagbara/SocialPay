using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Domain;
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
    public class SpectaOnboardingStagesService : ISpectaOnboardingStages
    {
        private readonly SocialPayDbContext _context;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;

        public SpectaOnboardingStagesService(SocialPayDbContext context, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _context = context;
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }
        public async Task<WebApiResponse> OnboardingStages(OnboardingStageRequestDto model)
        {
            try
            {
                var getregisteredinfo = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.email);
               
                if (getregisteredinfo != default)
                    return new WebApiResponse { ResponseCode = getregisteredinfo.RegistrationStatus, Message = "Success", StatusCode = ResponseCodes.Success };
              
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "No record found", StatusCode = ResponseCodes.InternalError };
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- OnboardingStages" + ex.ToString()}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request Failed", StatusCode = ResponseCodes.InternalError };
            }
        }
    }
}
