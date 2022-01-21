using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Services.Loan;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Merchant")]
    [ApiController]
    [Route("api/socialpay/loan")]
    public class LoanController : BaseController
    {
        private readonly LoanEligibiltyService _loanEligibiltyService;
        private readonly LoanRepaymentService _loanRepaymentService;
        private readonly ApplyForLoanService _applyForLoanService;

        public LoanController(LoanRepaymentService loanRepaymentService, LoanEligibiltyService loanEligibiltyService,
            ApplyForLoanService applyForLoanService,
          INotification notification) : base(notification)
        {
            _loanEligibiltyService = loanEligibiltyService ?? throw new ArgumentNullException(nameof(loanEligibiltyService));
            _loanRepaymentService = loanRepaymentService ?? throw new ArgumentNullException(nameof(loanRepaymentService));
            _applyForLoanService = applyForLoanService ?? throw new ArgumentNullException(nameof(applyForLoanService));
        }


        [HttpGet]
        [Route("available-loan-for-merchant")]
        public async Task<IActionResult> AvailableLoanForMerchant() => Response(await _loanEligibiltyService.MerchantEligibilty(User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [AllowAnonymous]
        [HttpGet]
        [Route("loan-repayment-details")]
        public async Task<IActionResult> LoanRepaymentModel() => Response(await _loanRepaymentService.GetRepaymentModel().ConfigureAwait(false));

        [HttpPost]
        [Route("apply-for-loan")]
        public async Task<IActionResult> CreateStore([FromBody] ApplyForloanRequestDTO request) => Response(await _applyForLoanService.ApplyForLoan(request, User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [HttpGet]
        [Route("loan-status")]
        public async Task<IActionResult> LoanStatus([FromQuery] long loanId) => Response(await _applyForLoanService.LoanStatus(loanId, User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [HttpPost]
        [Route("confirm-card-tokenization")]
        public async Task<IActionResult> ConfirmTokenization([FromBody] ConfirmTokenizationRequestDTO request) => Response(await _applyForLoanService.ConfirmTokenization(request, User.GetSessionDetails().ClientId).ConfigureAwait(false));
    }
}
