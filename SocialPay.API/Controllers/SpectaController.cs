using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Services.SpectaOnboardingService.Interface;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.API.Controllers
{
    [Route("api/socialpay/specta")]
    [ApiController]
    public class SpectaController : BaseController
    {
        private readonly ISpectaCustomerRegistration _spectaCustomerRegistration;
        private readonly ISpectaSendEmailVerificationCode _spectaSendEmailVerificationCode;
        private readonly ISpectaVerifyEmailConfirmationCode _spectaVerifyEmailConfirmationCode;
        public SpectaController(ISpectaCustomerRegistration spectaCustomerRegistration,
            ISpectaSendEmailVerificationCode spectaSendEmailVerificationCode,
            ISpectaVerifyEmailConfirmationCode spectaVerifyEmailConfirmationCode,
            INotification notification) : base(notification)
        {
            _spectaCustomerRegistration = spectaCustomerRegistration ?? throw new ArgumentNullException(nameof(spectaCustomerRegistration));
            _spectaSendEmailVerificationCode = spectaSendEmailVerificationCode ?? throw new ArgumentNullException(nameof(spectaSendEmailVerificationCode));
            _spectaVerifyEmailConfirmationCode = spectaVerifyEmailConfirmationCode ?? throw new ArgumentNullException(nameof(spectaVerifyEmailConfirmationCode));
        }

        [HttpPost]
        [Route("register-customer")]
        public async Task<IActionResult> CreateRegisterCustomerAsync([FromBody] RegisterCustomerRequestDto model) => Response(await _spectaCustomerRegistration.RegisterCustomer(model).ConfigureAwait(false));

        [HttpPost]
        [Route("send-email-verification-code")]
        public async Task<IActionResult> SendEmailVerificationCodeAsync([FromBody] SendEmailVerificationCodeRequestDto model) => Response(await _spectaSendEmailVerificationCode.SendEmailVerificationCode(model).ConfigureAwait(false));

        [HttpPost]
        [Route("verify-email-confirmation-code")]
        public async Task<IActionResult> VerifyEmailConfirmationCodeAsync([FromBody] VerifyEmailConfirmationCodeRequestDto model) => Response(await _spectaVerifyEmailConfirmationCode.VerifyEmailConfirmationCode(model).ConfigureAwait(false));
    }
}
