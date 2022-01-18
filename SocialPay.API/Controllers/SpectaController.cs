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
using System.Security.Claims;
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
        private readonly ISpectaLoggedInCustomerProfile _spectaLoggedInCustomerProfile;
        private readonly ISpectaAddOrrInformation _spectaAddOrrInformation;
        private readonly ISpectaBusinessSegmentAllList _spectaBusinessSegmentAllList;
        private readonly ISpectaRequestTicket _spectaRequestTicket;
        private readonly ISpectaConfirmTicket _spectaConfirmTicket;
        private readonly ISpectaOnboardingStages _spectaOnboardingStages;
        private readonly ISpectaCreateIndividualCurrentAccount _spectaCreateIndividualCurrentAccount;
        private readonly ISpectaSetDisbursementAccount _spectaSetDisbursementAccount;
        public SpectaController(ISpectaCustomerRegistration spectaCustomerRegistration,
            ISpectaSendEmailVerificationCode spectaSendEmailVerificationCode,
            ISpectaVerifyEmailConfirmationCode spectaVerifyEmailConfirmationCode,
            ISpectaSendBvnPhoneVerificationCode spectaSendBvnPhoneVerificationCode,
            ISpectaVerifyBvnPhoneConfirmationCode spectaVerifyBvnPhoneConfirmationCode,
            ISpectaAuthentication spectaAuthentication,
            ISpectaLoggedInCustomerProfile spectaLoggedInCustomerProfile,
            ISpectaAddOrrInformation spectaAddOrrInformation,
            ISpectaBusinessSegmentAllList spectaBusinessSegmentAllList,
            ISpectaRequestTicket spectaRequestTicket,
            ISpectaConfirmTicket spectaConfirmTicket,
            ISpectaOnboardingStages spectaOnboardingStages,
            ISpectaCreateIndividualCurrentAccount spectaCreateIndividualCurrentAccount,
            ISpectaSetDisbursementAccount spectaSetDisbursementAccount,
            INotification notification) : base(notification)
        {
            _spectaCustomerRegistration = spectaCustomerRegistration ?? throw new ArgumentNullException(nameof(spectaCustomerRegistration));
            _spectaSendEmailVerificationCode = spectaSendEmailVerificationCode ?? throw new ArgumentNullException(nameof(spectaSendEmailVerificationCode));
            _spectaVerifyEmailConfirmationCode = spectaVerifyEmailConfirmationCode ?? throw new ArgumentNullException(nameof(spectaVerifyEmailConfirmationCode));
            _spectaSendBvnPhoneVerificationCode = spectaSendBvnPhoneVerificationCode ?? throw new ArgumentNullException(nameof(spectaSendBvnPhoneVerificationCode));
            _spectaVerifyBvnPhoneConfirmationCode = spectaVerifyBvnPhoneConfirmationCode ?? throw new ArgumentNullException(nameof(spectaVerifyBvnPhoneConfirmationCode));
            _spectaAuthentication = spectaAuthentication ?? throw new ArgumentNullException(nameof(spectaAuthentication));
            _spectaLoggedInCustomerProfile = spectaLoggedInCustomerProfile ?? throw new ArgumentNullException(nameof(spectaLoggedInCustomerProfile));
            _spectaAddOrrInformation = spectaAddOrrInformation ?? throw new ArgumentNullException(nameof(spectaAddOrrInformation));
            _spectaBusinessSegmentAllList = spectaBusinessSegmentAllList ?? throw new ArgumentNullException(nameof(spectaBusinessSegmentAllList));
            _spectaConfirmTicket = spectaConfirmTicket ?? throw new ArgumentNullException(nameof(spectaConfirmTicket));
            _spectaRequestTicket = spectaRequestTicket ?? throw new ArgumentNullException(nameof(spectaRequestTicket));
            _spectaOnboardingStages = spectaOnboardingStages ?? throw new ArgumentNullException(nameof(spectaOnboardingStages));
            _spectaCreateIndividualCurrentAccount = spectaCreateIndividualCurrentAccount ?? throw new ArgumentNullException(nameof(spectaCreateIndividualCurrentAccount));
            _spectaSetDisbursementAccount = spectaSetDisbursementAccount ?? throw new ArgumentNullException(nameof(spectaSetDisbursementAccount));
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

        [HttpGet]
        [Route("logged-in-customer-profile")]
        public async Task<IActionResult> LoggedInCustomerProfileAsync() => Response(await _spectaLoggedInCustomerProfile.LoggedInCustomerProfile(User.FindFirstValue(ClaimTypes.Email)).ConfigureAwait(false));
        [HttpPost]
        [Route("add-orr-information")]
        public async Task<IActionResult> AddOrrInformationAsync([FromBody] AddOrrInformationRequestDto model) => Response(await _spectaAddOrrInformation.AddOrrInformation(model, User.FindFirstValue(ClaimTypes.Email)).ConfigureAwait(false));

        [HttpGet]
        [Route("business-segment-all-list")]
        public async Task<IActionResult> BusinessSegmentAllListAsync() => Response(await _spectaBusinessSegmentAllList.BusinessSegmentAllList(User.FindFirstValue(ClaimTypes.Email)).ConfigureAwait(false));

        [HttpPost]
        [Route("request-ticket")]
        public async Task<IActionResult> RequestTicketAsync([FromBody] RequestTicketDto model) => Response(await _spectaRequestTicket.RequestTicket(model, User.FindFirstValue(ClaimTypes.Email)).ConfigureAwait(false));

        [HttpPost]
        [Route("confirm-ticket")]
        public async Task<IActionResult> ConfirmTicketAsyn([FromBody] ConfirmTicketRequestDto model) => Response(await _spectaConfirmTicket.ConfirmTicket(model, User.FindFirstValue(ClaimTypes.Email)).ConfigureAwait(false));

        [HttpPost]
        [Route("create-individual-current-account")]
        public async Task<IActionResult> CreateIndividualCurrentAccountAsync([FromForm] CreateIndividualCurrentAccountRequestDto model) => Response(await _spectaCreateIndividualCurrentAccount.CreateIndividualCurrentAccount(model, User.FindFirstValue(ClaimTypes.Email)).ConfigureAwait(false));

        [HttpPost]
        [Route("set-disbursement-account")]
        public async Task<IActionResult> DisbursementAccountAsync([FromBody] SetDisbursementAccountRequestDto model) => Response(await _spectaSetDisbursementAccount.SetDisbursementAccount(model, User.FindFirstValue(ClaimTypes.Email)).ConfigureAwait(false));

        [HttpPost]
        [Route("specta-onboarding-stages")]
        public async Task<IActionResult> OnboardingStagesAsync([FromBody] OnboardingStageRequestDto model) => Response(await _spectaOnboardingStages.OnboardingStages(model).ConfigureAwait(false));
    }
}
