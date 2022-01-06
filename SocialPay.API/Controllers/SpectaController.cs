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
        private readonly ISpectaSendBvnPhoneVerificationCode _spectaSendBvnPhoneVerificationCode;
        private readonly ISpectaVerifyBvnPhoneConfirmationCode _spectaVerifyBvnPhoneConfirmationCode;
        private readonly ISpectaAuthentication _spectaAuthentication;
       // private readonly ISpectaLoggedInCustomerProfile _spectaLoggedInCustomerProfile;
        public SpectaController(ISpectaCustomerRegistration spectaCustomerRegistration,
            ISpectaSendEmailVerificationCode spectaSendEmailVerificationCode,
            ISpectaVerifyEmailConfirmationCode spectaVerifyEmailConfirmationCode,
            ISpectaSendBvnPhoneVerificationCode spectaSendBvnPhoneVerificationCode,
            ISpectaVerifyBvnPhoneConfirmationCode spectaVerifyBvnPhoneConfirmationCode,
            ISpectaAuthentication spectaAuthentication,
            //ISpectaLoggedInCustomerProfile spectaLoggedInCustomerProfile,
            INotification notification) : base(notification)
        {
            _spectaCustomerRegistration = spectaCustomerRegistration ?? throw new ArgumentNullException(nameof(spectaCustomerRegistration));
            _spectaSendEmailVerificationCode = spectaSendEmailVerificationCode ?? throw new ArgumentNullException(nameof(spectaSendEmailVerificationCode));
            _spectaVerifyEmailConfirmationCode = spectaVerifyEmailConfirmationCode ?? throw new ArgumentNullException(nameof(spectaVerifyEmailConfirmationCode));
            _spectaSendBvnPhoneVerificationCode = spectaSendBvnPhoneVerificationCode ?? throw new ArgumentNullException(nameof(spectaSendBvnPhoneVerificationCode));
            _spectaVerifyBvnPhoneConfirmationCode = spectaVerifyBvnPhoneConfirmationCode ?? throw new ArgumentNullException(nameof(spectaVerifyBvnPhoneConfirmationCode));
            _spectaAuthentication = spectaAuthentication ?? throw new ArgumentNullException(nameof(spectaAuthentication));
           // _spectaLoggedInCustomerProfile = spectaLoggedInCustomerProfile ?? throw new ArgumentNullException(nameof(spectaLoggedInCustomerProfile));
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

        [HttpPost]
        [Route("send-bvn-phone-verification-code")]
        public async Task<IActionResult> SendBvnPhoneVerificationCodeAsync([FromForm] SendBvnPhoneVerificationCodeRequestDto model) => Response(await _spectaSendBvnPhoneVerificationCode.SendBvnPhoneVerificationCode(model).ConfigureAwait(false));

        [HttpPost]
        [Route("verify-bvn-phone-confirmation-code")]
        public async Task<IActionResult> VerifyBvnPhoneConfirmationCodeAsync([FromBody] VerifyBvnPhoneConfirmationCodeRequestDto model) => Response(await _spectaVerifyBvnPhoneConfirmationCode.VerifyBvnPhoneConfirmationCode(model).ConfigureAwait(false));

        [HttpPost]
        [Route("authenticate-user")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateRequestDto model) => Response(await _spectaAuthentication.Authenticate(model).ConfigureAwait(false));

        //[HttpGet]
        //[Route("logged-in-customer-profile")]
        //public async Task<IActionResult> LoggedInCustomerProfileAsync() => Response(await _spectaLoggedInCustomerProfile.LoggedInCustomerProfile(User.FindFirstValue(ClaimTypes.Email)).ConfigureAwait(false));
    }
}
