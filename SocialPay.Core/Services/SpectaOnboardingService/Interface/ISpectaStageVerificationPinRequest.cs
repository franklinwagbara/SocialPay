using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.ISpectaOnboardingService
{
    public interface ISpectaStageVerificationPinRequest
    {
        public Task<WebApiResponse> SpectaStageVerificationPinRequest(SpectaStageVerificationPinRequestDto model);
        public Task<WebApiResponse> SpectaPinConfirmationRequest(SpectaPinConfirmationRequestDto model);
    }
}
