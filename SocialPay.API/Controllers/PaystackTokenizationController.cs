using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Account;
using SocialPay.Core.Services.Authentication;
using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Core.Services.Specta;
using SocialPay.Core.Services.SpectaOnboardingService.Services;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialPay.API.Controllers
{
    [Route("api/socialpay/Tokenization")]
    [ApiController]
    public class PaystackTokenizationController : BaseController
    {

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AccountsController));
        private readonly ISpectaChargeCard _spectaChargeCard;
        private readonly ISpectaSendPhone _spectaSendPhone;
        private readonly ISpectaSendOtp _spectaSendOtp;
        private readonly ISpectaSendPin _spectaSendPin;
        private readonly ISpectaValidateCharge _spectaValidateCharge;


        public PaystackTokenizationController(ISpectaChargeCard spectaChargeCard, ISpectaSendPhone spectaSendPhone, ISpectaSendOtp spectaSendOtp,
            ISpectaSendPin spectaSendPin, ISpectaValidateCharge spectaValidateCharge, INotification notification) : base(notification)

        {
            _spectaValidateCharge = spectaValidateCharge ?? throw new ArgumentNullException(nameof(spectaValidateCharge));
            _spectaSendPin = spectaSendPin ?? throw new ArgumentNullException(nameof(spectaSendPin));
            _spectaSendOtp = spectaSendOtp ?? throw new ArgumentNullException(nameof(spectaSendOtp));
            _spectaSendPhone = spectaSendPhone ?? throw new ArgumentNullException(nameof(spectaSendPhone));
            _spectaChargeCard = spectaChargeCard ?? throw new ArgumentNullException(nameof(spectaChargeCard));
        }

        [HttpPost]
        [Route("charge-card")]
        public async Task<IActionResult> ChargeCardAsync([FromBody] ChargeCardRequestDto model) => Response(await _spectaChargeCard.ChargeCard(model).ConfigureAwait(false));

        [HttpPost]
        [Route("send-phone")]
        public async Task<IActionResult> SendPhoneAsync([FromBody] SendPhoneRequestDto model) => Response(await _spectaSendPhone.SendPhone(model).ConfigureAwait(false));

        [HttpPost]
        [Route("send-otp")]
        public async Task<IActionResult> SendOtpAsync([FromBody] SendOtpRequestDto model) => Response(await _spectaSendOtp.SendOtp(model).ConfigureAwait(false));

        [HttpPost]
        [Route("send-pin")]
        public async Task<IActionResult> SendPinAsync([FromBody] SendPinRequestDto model) => Response(await _spectaSendPin.SendPin(model).ConfigureAwait(false));

        [HttpPost]
        [Route("validate-charge")]
        public async Task<IActionResult> ValidateChargeAsync([FromBody] ValidateChargeRequestDto model) => Response(await _spectaValidateCharge.ValidateCharge(model).ConfigureAwait(false));

    }
}
