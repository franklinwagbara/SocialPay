﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Messaging.SendGrid;
using SocialPay.Core.Repositories.UserService;
using SocialPay.Core.Services.Account;
using SocialPay.Core.Services.Authentication;
using SocialPay.Core.Services.Customer;
using SocialPay.Core.Services.Loan;
using SocialPay.Core.Services.Merchant;
using SocialPay.Core.Services.Merchant.Interfaces;
using SocialPay.Core.Services.Report;
using SocialPay.Core.Services.Store;
using SocialPay.Core.Services.Wallet;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.Notification;

namespace SocialPay.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Super Administrator")]
    [Route("api/socialpay/admin")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly ADRepoService _aDRepoService;
        private readonly AuthRepoService _authRepoService;
        private readonly MerchantReportService _merchantReportService;
        private readonly TransactionService _transactionService;
        private readonly UserRepoService _userRepoService;
        private readonly CustomerRepoService _customerRepoService;
        private readonly CreateMerchantWalletService _createMerchantWalletService;
        private readonly SendGridEmailService _sendGridEmailService;
        private readonly CreateBulkMerchantService _createBulkMerchantService;
        private readonly StoreReportRepository _storeReportRepository;
        private readonly ApplyForLoanService _applyForLoanService;
        private readonly LoanEligibiltyService _loanEligibiltyService;
        private readonly LoanRepaymentService _loanRepaymentService;
        private readonly IMerchantsWithOutPaymentLink _merchantsWithOutPaymentLink;
        private readonly IMerchantCustomerTransactions _merchantCustomerTransactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AdminController));

        public AdminController(ADRepoService aDRepoService, MerchantReportService merchantReportService,
            TransactionService transactionService, AuthRepoService authRepoService,
            CreateMerchantWalletService createMerchantWalletService,
            UserRepoService userRepoService, CustomerRepoService customerRepoService,
            SendGridEmailService sendGridEmailService, CreateBulkMerchantService createBulkMerchantService,
            StoreReportRepository storeReportRepository, ApplyForLoanService applyForLoanService,
            LoanEligibiltyService loanEligibiltyService, LoanRepaymentService loanRepaymentService,
            IMerchantsWithOutPaymentLink merchantsWithOutPaymentLink,
            IMerchantCustomerTransactions merchantCustomerTransactions,
             INotification notification) : base(notification)
        {
            _aDRepoService = aDRepoService;
            _merchantReportService = merchantReportService;
            _transactionService = transactionService;
            _authRepoService = authRepoService;
            _createMerchantWalletService = createMerchantWalletService;
            _userRepoService = userRepoService;
            _customerRepoService = customerRepoService;
            _sendGridEmailService = sendGridEmailService;
            _createBulkMerchantService = createBulkMerchantService ?? throw new ArgumentNullException(nameof(createBulkMerchantService));
            _storeReportRepository = storeReportRepository ?? throw new ArgumentNullException(nameof(storeReportRepository));
            _applyForLoanService = applyForLoanService ?? throw new ArgumentNullException(nameof(applyForLoanService));
            _loanEligibiltyService = loanEligibiltyService ?? throw new ArgumentNullException(nameof(loanEligibiltyService));
            _loanRepaymentService = loanRepaymentService ?? throw new ArgumentNullException(nameof(loanRepaymentService));
            _merchantsWithOutPaymentLink = merchantsWithOutPaymentLink ?? throw new ArgumentNullException(nameof(merchantsWithOutPaymentLink));
            _merchantCustomerTransactions = merchantCustomerTransactions ?? throw new ArgumentNullException(nameof(merchantCustomerTransactions));
        }

        [HttpPost]
        [Route("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto model)
        {
            _log4net.Info("Tasks starts to create account" + " | " + model.Username + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _aDRepoService.RegisterUser(model);
                    return Ok(result);
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + model.Username + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [HttpPost]
        [Route("update-user-account")]
        public async Task<IActionResult> UpdateUserAccount([FromBody] UpdateUserRequestDto model)
        {
            _log4net.Info("Tasks starts to disable account" + " | " + model.Email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _authRepoService.ModifyUserAccount(model);
                    return Ok(result);
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + model.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }

        //[HttpPost]
        //[Route("resend-guest-account-details")]
        //public async Task<IActionResult> SendGuestCredentials([FromBody] GuestAccountRequestDto model)
        //{
        //    _log4net.Info("Tasks starts to disable account" + " | " + model.Email + " | " + DateTime.Now);

        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var identity = User.Identity as ClaimsIdentity;
        //            var clientName = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        //            var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        //            var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        //            var result = await _userRepoService.ResendGuestAccountDetails(model, Convert.ToInt32(clientId));
        //            return Ok(result);
        //        }
        //        var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage));
        //        response.ResponseCode = AppResponseCodes.Failed;
        //        response.Data = message;
        //        return BadRequest(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        _log4net.Error("Error occured" + " | " + model.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);
        //        response.ResponseCode = AppResponseCodes.InternalError;
        //        return BadRequest(response);
        //    }
        //}

        [HttpGet]
        [Route("get-all-applied-loan")]
        public async Task<IActionResult> GetAppliedLoans() => Response(await _applyForLoanService.GetAllAppliedLoan(User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [HttpGet]
        [Route("get-merchant-loan-eligibility")]
        public async Task<IActionResult> GetMerchantLoanEligibility([FromQuery] long clientId) => Response(await _loanEligibiltyService.MerchantEligibilty(clientId).ConfigureAwait(false));

        [HttpPost]
        [Route("approve-loan")]
        public async Task<IActionResult> ApproveLoan([FromBody] AdminLoanApproverRequestDTO model) => Response(await _applyForLoanService.ApproveLoan(model, User.GetSessionDetails().ClientId, User.GetSessionDetails().Email).ConfigureAwait(false));

        [HttpPost]
        [Route("create-loan-repayment-model")]
        public async Task<IActionResult> LoanRepaymentPlan([FromBody] LoanRepaymentRequestDto model) => Response(await _loanRepaymentService.CreateRepaymentModel(model, User.GetSessionDetails().ClientId, User.GetSessionDetails().Email).ConfigureAwait(false));

        [HttpPost]
        [Route("Delete-loan-repayment-model")]
        public async Task<IActionResult> DeleteLoanRepayment([FromBody] DeleteLoanRepaymentRequestDto model) => Response(await _loanRepaymentService.DeleteRepaymentModel(model, User.GetSessionDetails().ClientId, User.GetSessionDetails().Email).ConfigureAwait(false));

        [HttpPost]
        [Route("unlock-user-account")]
        public async Task<IActionResult> UnLockAccount([FromBody] UpdateUserRequestDto model)
        {
            _log4net.Info("Tasks starts to unlock account" + " | " + model.Email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var email = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (ModelState.IsValid)
                    return Ok(await _authRepoService.UnlockUserAccount(model, Convert.ToInt32(clientId), email));

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;

                return BadRequest(response);

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + model.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Route("get-merchants")]
        public async Task<IActionResult> GetMerchants([FromQuery] bool hasCompanyProfile)
        {
            // _log4net.Info("Tasks starts to create account" + " | " + model.Username + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                return Ok(await _merchantReportService.GetMerchants(hasCompanyProfile));
            }
            catch (Exception)
            {
                // _log4net.Error("Error occured" + " | " + model.Username + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;

                return BadRequest(response);
            }
        }

        [HttpGet]
        [Route("list-of-merchants-without-payment-link")]
        public async Task<IActionResult> MerchantsWithOutPaymentLinkAsync()
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {

                    return Ok(await _merchantsWithOutPaymentLink.MerchantsWithOutPaymentLink());
                }

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;

                return StatusCode(500, response);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("customer-transactions")]
        public async Task<IActionResult> CustomerTransactionsAsync()
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {

                    return Ok(await _merchantCustomerTransactions.CustomerTransactions());
                }

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;

                return StatusCode(500, response);
            }
        }


        //[AllowAnonymous]
        [HttpGet]
        [Route("get-store-merchants")]
        public async Task<IActionResult> GetMerchantsWithStore()
        {
            // _log4net.Info("Tasks starts to create account" + " | " + model.Username + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                return Ok(await _storeReportRepository.GetMerchantWithStoreInfoAsync(User.GetSessionDetails()));
            }
            catch (Exception)
            {
                // _log4net.Error("Error occured" + " | " + model.Username + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;

                return BadRequest(response);
            }
        }

        //[AllowAnonymous]
        [HttpGet]
        [Route("get-transactions")]
        public async Task<IActionResult> GetCustomerTransactions([FromQuery] string category)
        {
            // _log4net.Info("Tasks starts to create account" + " | " + model.Username + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    return Ok(await _transactionService.GetCustomerOrders(category));
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                // _log4net.Error("Error occured" + " | " + model.Username + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }

        //[AllowAnonymous]
        [HttpGet]
        [Route("get-user-signup-status")]
        public async Task<IActionResult> GetSignUpStatus()
        {
            // _log4net.Info("Tasks starts to create account" + " | " + model.Username + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                return Ok(await _transactionService.GetOnboardingJourney());
                //if (ModelState.IsValid)
                //{
                //    var result = await _transactionService.GetOnboardingJourney();
                //    return Ok(result);
                //}
                //var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                //    .Select(e => e.ErrorMessage));
                //response.ResponseCode = AppResponseCodes.Failed;
                //response.Data = message;
                //return BadRequest(response);

            }
            catch (Exception ex)
            {
                // _log4net.Error("Error occured" + " | " + model.Username + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [HttpGet]
        [Route("admin-get-customers-transaction-count")]
        public async Task<IActionResult> AdminGetCustomersTransactionCount([FromQuery] string category)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                    return Ok(await _merchantReportService.AdminGetCustomersTransactionCount(category));

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;

                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route("admin-get-customers-transaction-value")]
        public async Task<IActionResult> AdminGetCustomersTransactionValue([FromQuery] string category)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                    return Ok(await _merchantReportService.AdminGetCustomersTransactionValue(category));

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;

                return StatusCode(500, response);
            }
        }



        [HttpGet]
        [Route("top-used-payment-channel")]
        public async Task<IActionResult> TopUsedPaymentChannel()
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {

                    return Ok(await _merchantReportService.AdminMostUsedPaymentChannel());

                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;

                return StatusCode(500, response);
            }
        }

      //  [AllowAnonymous]
        [HttpPost]
        [Route("create-multiple-merchant")]
        public async Task<IActionResult> CreateMultipleMerchant(IFormFile doc)
        {
            // _log4net.Info("Tasks starts to unlock account" + " | " + model.Email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {

                if (ModelState.IsValid)
                    return Ok(await _createBulkMerchantService.BulkCreateMerchant(doc));

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                //  _log4net.Error("Error occured" + " | " + model.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [HttpPost]
        [Route("create-multiple-merchant-business-info")]
        public async Task<IActionResult> CreateMultipleMerchantBusinessInfo(IFormFile doc)
        {
            // _log4net.Info("Tasks starts to unlock account" + " | " + model.Email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                    return Ok(await _createBulkMerchantService.BulkCreateMerchantBusinessInfo(doc));

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;

                return BadRequest(response);

            }
            catch (Exception ex)
            {
                //  _log4net.Error("Error occured" + " | " + model.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("create-multiple-merchant-bank-info")]
        public async Task<IActionResult> CreateMultipleMerchantBankInfo(IFormFile doc)
        {
            // _log4net.Info("Tasks starts to unlock account" + " | " + model.Email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {

                if (ModelState.IsValid)
                    return Ok(await _createBulkMerchantService.BulkCreateMerchantBankInfo(doc));

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                //  _log4net.Error("Error occured" + " | " + model.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        ////[AllowAnonymous]
        ////[HttpGet]
        ////[Route("clear-user-account")]
        ////public async Task<IActionResult> ClearUserDetails(string email, string reference)
        ////{
        ////    _log4net.Info("Tasks starts to clear user account" + " | " + email + " | " + DateTime.Now);

        ////    var response = new WebApiResponse { };
        ////    try
        ////    {
        ////        if (reference != "7467r")
        ////            return BadRequest();

        ////        if (ModelState.IsValid)
        ////        {
        ////            var result = await _transactionService.ClearUserAccount(email);
        ////            return Ok(result);
        ////        }
        ////        var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        ////            .Select(e => e.ErrorMessage));
        ////        response.ResponseCode = AppResponseCodes.Failed;
        ////        response.Data = message;
        ////        return BadRequest(response);

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        _log4net.Error("Error occured" + " | " + email + " | " + ex.Message.ToString() + " | " + DateTime.Now);
        ////        response.ResponseCode = AppResponseCodes.InternalError;
        ////        return BadRequest(response);
        ////    }
        ////}



        ////[AllowAnonymous]
        ////[HttpPost]
        ////[Route("clear-user-wallet")]
        ////public async Task<IActionResult> ClearMerchantWallet(string phoneNumber, string reference)
        ////{
        ////    _log4net.Info("Tasks starts to clear user account" + " | " + phoneNumber + " | " + DateTime.Now);

        ////    var response = new WebApiResponse { };
        ////    try
        ////    {
        ////        if (reference != "12345gh2")
        ////            return BadRequest();

        ////        if (ModelState.IsValid)
        ////        {
        ////            var result = await _createMerchantWalletService.ClearMerchantWalletInfo(phoneNumber);
        ////            return Ok(result);
        ////        }
        ////        var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        ////            .Select(e => e.ErrorMessage));
        ////        response.ResponseCode = AppResponseCodes.Failed;
        ////        response.Data = message;
        ////        return BadRequest(response);

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        _log4net.Error("Error occured" + " | " + phoneNumber + " | " + ex.Message.ToString() + " | " + DateTime.Now);
        ////        response.ResponseCode = AppResponseCodes.InternalError;
        ////        return BadRequest(response);
        ////    }
        ////}


        [HttpGet]
        [Route("get-all-logged-disputes")]
        public async Task<IActionResult> GetLoggedDisputes()
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var identity = User.Identity as ClaimsIdentity;
                    var clientName = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    var result = await _merchantReportService.GetAllLoggedDisputes(Convert.ToInt32(clientId), true);
                    return Ok(result);
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("get-all-users")]
        public async Task<IActionResult> Getusers([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "45fth1")
                    return BadRequest();

                if (ModelState.IsValid)
                {

                    var result = await _merchantReportService.GetAllUsers();
                    return Ok(result);
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("get-all-transaction-logs")]
        public async Task<IActionResult> GetTransactionLogAsync([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "34sfg2e")
                    return BadRequest();

                if (ModelState.IsValid)
                {

                    var result = await _merchantReportService.GetAllTransactions();
                    return Ok(result);
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("get-all-failed-transaction-logs")]
        public async Task<IActionResult> GetFailedTransactionLogAsync([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "sterling120f14")
                    return BadRequest();

                if (ModelState.IsValid)
                {

                    var result = await _merchantReportService.GetAllFailedTransactions();
                    return Ok(result);
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        ////[AllowAnonymous]
        ////[HttpGet]
        ////[Route("get-all-payment-response-logs")]
        ////public async Task<IActionResult> GetPaymentResponseLogsAsync([FromQuery] string reference)
        ////{
        ////    var response = new WebApiResponse { };
        ////    try
        ////    {
        ////        if (reference != "sterling0k1v8g1")
        ////            return BadRequest();

        ////        if (ModelState.IsValid)
        ////        {
        ////            return Ok(await _merchantReportService.GetAllPaymentResponseLogs());
        ////        }
        ////        var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        ////            .Select(e => e.ErrorMessage));
        ////        response.ResponseCode = AppResponseCodes.Failed;
        ////        response.Data = message;
        ////        return BadRequest(response);

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        response.ResponseCode = AppResponseCodes.InternalError;
        ////        return BadRequest(response);
        ////    }
        ////}


        [AllowAnonymous]
        [HttpGet]
        [Route("get-all-default-merchant-wallet-logs")]
        public async Task<IActionResult> GetDefaultWalletLogs([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "453gy9s")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.GetAllDefaultWalletLogsAsync());
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("get-all-default-merchant-wallet-transfer-request -logs")]
        public async Task<IActionResult> GetDefaultWalletTransferLogs([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "43rt")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.GetAllDefaultWalletTransferRequestLogsAsync());
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("clear-default-log")]
        public async Task<IActionResult> ClearDefaultLogs([FromQuery] string reference, string paymentRefernce)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "t645")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.ClearDefaultLogs(paymentRefernce));
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }



        [AllowAnonymous]
        [HttpGet]
        [Route("get-card-requests")]
        public async Task<IActionResult> GetCardTrans([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "4hj2")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.FioranoCardRequest());
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("get-card-requests-default")]
        public async Task<IActionResult> GetCardTransDefault([FromQuery] string reference, string paymentRefernce)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "h128")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.ClearFioranoCardRequest(paymentRefernce));
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        //[AllowAnonymous]
        //[HttpGet]
        //[Route("get-all-merchant-bank-info")]
        //public async Task<IActionResult> GetMerchantBankInfo([FromQuery] string reference)
        //{
        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (reference != "sterling2v@m11")
        //            return BadRequest();

        //        if (ModelState.IsValid)
        //        {
        //            return Ok(await _merchantReportService.GetMerchantBankInfoAsync());
        //        }
        //        var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage));
        //        response.ResponseCode = AppResponseCodes.Failed;
        //        response.Data = message;
        //        return BadRequest(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        response.ResponseCode = AppResponseCodes.InternalError;
        //        return BadRequest(response);
        //    }
        //}


        [AllowAnonymous]
        [HttpGet]
        [Route("get-all-merchant-business-info")]
        public async Task<IActionResult> GetMerchantBusinessInfo([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "73jf5")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.GetMerchantBusinessInfoAsync());
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("get-all-interbank-default-info")]
        public async Task<IActionResult> GetInterBankRequest([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "73hf")
                    return BadRequest();

                if (ModelState.IsValid)
                    return Ok(await _merchantReportService.GetInterBankRequestAsync());

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;

                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("get-all-non-escrow-trans")]
        public async Task<IActionResult> GetTransactions([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "09hgx21")
                    return BadRequest();

                return Ok(await _merchantReportService.GetNonEscrowBankTransactions());

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("get-bank-non-escrow-trans")]
        public async Task<IActionResult> GetInterbankRequest([FromQuery] string reference, string paymentRefernce)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "07gr")
                    return BadRequest();

                return Ok(await _merchantReportService.ModifyFioranoRequestInfo(paymentRefernce));

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("get-user-trans")]
        public async Task<IActionResult> GetFioranoTransactions([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "334fds2")
                    return BadRequest();

                return Ok(await _merchantReportService.GetFioranoTransactions());

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }



        //[AllowAnonymous]
        //[HttpGet]
        //[Route("get-payment-links")]
        //public async Task<IActionResult> PaymentLinks([FromQuery] string reference)
        //{
        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (reference != "43455")
        //            return BadRequest();

        //        return Ok(await _merchantReportService.GetPaymentLinks());

        //    }
        //    catch (Exception ex)
        //    {
        //        response.ResponseCode = AppResponseCodes.InternalError;
        //        return BadRequest(response);
        //    }
        //}



        [AllowAnonymous]
        [HttpGet]
        [Route("get-interbank-lnterBankRequest")]
        public async Task<IActionResult> InterbankRequest([FromQuery] string reference, string merchant)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "4g82")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.RemoveInterbankRequestInfo(merchant));
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }



        [AllowAnonymous]
        [HttpGet]
        [Route("get-Customer-OtherPayment")]
        public async Task<IActionResult> GetCustomerOtherPayment([FromQuery] string reference)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "jur673")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.GetCustomerOtherTransactionInfo());
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }



        ////[AllowAnonymous]
        ////[HttpGet]
        ////[Route("get-Wallet-Info")]
        //////public async Task<IActionResult> GetWalletInfo([FromQuery] string reference)
        //////{
        //////    var response = new WebApiResponse { };
        //////    try
        //////    {
        //////        if (reference != "57h34")
        //////            return BadRequest();

        //////        if (ModelState.IsValid)
        //////        {
        //////            return Ok(await _merchantReportService.GetWalletInfo());
        //////        }
        //////        var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        //////            .Select(e => e.ErrorMessage));

        //////        response.ResponseCode = AppResponseCodes.Failed;
        //////        response.Data = message;

        //////        return BadRequest(response);

        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        response.ResponseCode = AppResponseCodes.InternalError;
        //////        return BadRequest(response);
        //////    }
        //////}



        [AllowAnonymous]
        [HttpGet]
        [Route("get-latest-OtherPayment")]
        public async Task<IActionResult> GetMerchantDetails([FromQuery] string reference, string merchant)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "t562")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.ValidateMerchantInfo(merchant));
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }



        [AllowAnonymous]
        [HttpGet]
        [Route("get-updated-trans")]
        public async Task<IActionResult> UpdateTransLasync([FromQuery] string reference, string paymentRef, string code)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "4345d")
                    return BadRequest();

                if (ModelState.IsValid)
                    return Ok(await _merchantReportService.UpdateTransLog(paymentRef, code));

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("get-updated-trans-local")]
        public async Task<IActionResult> UpdateTransLocalAync([FromQuery] string reference, string paymentRef, string code)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "435g2")
                    return BadRequest();

                if (ModelState.IsValid)
                    return Ok(await _merchantReportService.UpdateCustomerInfo2(paymentRef, code));

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("get-payment-details")]
        public async Task<IActionResult> ValidatePayment([FromQuery] string reference, string merchant)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "3dd22")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.ValidateInfo(merchant));
                }

                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;

                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("get-user-wallet-info")]
        public async Task<IActionResult> ValidateuserWallet([FromQuery] string reference, string merchant)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "gh3219")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.ValidateWalletInfo(merchant));
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }



        [AllowAnonymous]
        [HttpGet]
        [Route("validate-user-wallet-info")]
        public async Task<IActionResult> UpdateInfo([FromQuery] string reference, string merchant)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "r42g")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.UpdateWalletInfo(merchant));
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            } 
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        ////[AllowAnonymous]
        ////[HttpGet]
        ////[Route("get-request-details")]
        ////public async Task<IActionResult> ValidateRequests([FromQuery] string reference, string merchant)
        ////{
        ////    var response = new WebApiResponse { };
        ////    try
        ////    {
        ////        if (reference != "34df12")
        ////            return BadRequest();

        ////        if (ModelState.IsValid)
        ////        {
        ////            return Ok(await _merchantReportService.InterRequestAsync(merchant));
        ////        }
        ////        var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        ////            .Select(e => e.ErrorMessage));
        ////        response.ResponseCode = AppResponseCodes.Failed;
        ////        response.Data = message;
        ////        return BadRequest(response);

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        response.ResponseCode = AppResponseCodes.InternalError;
        ////        return BadRequest(response);
        ////    }
        ////}

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("get-admin-transactions-default")]
        //public async Task<IActionResult> GetCustomerTransactionsAdmin([FromQuery] string category, string reference)
        //{
        //    if (reference != "12vg345")
        //        return BadRequest();

        //    var response = new WebApiResponse { };

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            return Ok(await _transactionService.GetCustomerOrders(category));
        //        }
        //        var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage));

        //        response.ResponseCode = AppResponseCodes.Failed;
        //        response.Data = message;

        //        return BadRequest(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        response.ResponseCode = AppResponseCodes.InternalError;

        //        return BadRequest(response);
        //    }
        //}


        ////[AllowAnonymous]
        ////[HttpGet]
        ////[Route("send-test-email")]
        ////public async Task<IActionResult> SendEmail([FromQuery] string email)
        ////{

        ////    var response = new WebApiResponse { };

        ////    try
        ////    {
        ////        if (ModelState.IsValid)
        ////        {

        ////            var mailBuilder = new StringBuilder();
        ////            mailBuilder.AppendLine("Dear" + " " + email + "," + "<br />");
        ////            mailBuilder.AppendLine("<br />");
        ////            mailBuilder.AppendLine("You have successfully sign up. Please confirm your sign up by clicking the link below.<br />");
        ////            mailBuilder.AppendLine("Kindly use this token" + "  " + "88434" + "  " + "and" + " " + "www" + "<br />");
        ////            // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
        ////            mailBuilder.AppendLine("Best Regards,");

        ////            return Ok(await _sendGridEmailService.SendMail(mailBuilder.ToString(), email));
        ////        }
        ////        var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        ////            .Select(e => e.ErrorMessage));

        ////        response.ResponseCode = AppResponseCodes.Failed;
        ////        response.Data = message;

        ////        return BadRequest(response);

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        response.ResponseCode = AppResponseCodes.InternalError;

        ////        return BadRequest(response);
        ////    }
        ////}

    }
}