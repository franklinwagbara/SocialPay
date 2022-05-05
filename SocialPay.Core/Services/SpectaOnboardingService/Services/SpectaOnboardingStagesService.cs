using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaOnboardingStagesService : ISpectaOnboardingStages
    {
        private readonly SocialPayDbContext _context;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaOnboardingStagesService));

        public SpectaOnboardingStagesService(SocialPayDbContext context)
        {
            _context = context;
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
                _log4net.Error("Error occured" + " | " + "OnboardingStages" + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request Failed", StatusCode = ResponseCodes.InternalError };
            }
        }
    }
}
