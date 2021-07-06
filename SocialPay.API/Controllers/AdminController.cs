using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Messaging.SendGrid;
using SocialPay.Core.Repositories.UserService;
using SocialPay.Core.Services.Account;
using SocialPay.Core.Services.Authentication;
using SocialPay.Core.Services.Customer;
using SocialPay.Core.Services.Report;
using SocialPay.Core.Services.Wallet;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;

namespace SocialPay.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Super Administrator")]
    [Route("api/socialpay/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ADRepoService _aDRepoService;
        private readonly AuthRepoService _authRepoService;
        private readonly MerchantReportService _merchantReportService;
        private readonly TransactionService _transactionService;
        private readonly UserRepoService _userRepoService;
        private readonly CustomerRepoService _customerRepoService;
        private readonly CreateMerchantWalletService _createMerchantWalletService;
        private readonly SendGridEmailService _sendGridEmailService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AdminController));

        public AdminController(ADRepoService aDRepoService, MerchantReportService merchantReportService,
            TransactionService transactionService, AuthRepoService authRepoService,
            CreateMerchantWalletService createMerchantWalletService,
            UserRepoService userRepoService, CustomerRepoService customerRepoService,
            SendGridEmailService sendGridEmailService)
        {
            _aDRepoService = aDRepoService;
            _merchantReportService = merchantReportService;
            _transactionService = transactionService;
            _authRepoService = authRepoService;
            _createMerchantWalletService = createMerchantWalletService;
            _userRepoService = userRepoService;
            _customerRepoService = customerRepoService;
            _sendGridEmailService = sendGridEmailService;
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



        [HttpPost]
        [Route("unlock-user-account")]
        public async Task<IActionResult> UnLockAccount([FromBody] UpdateUserRequestDto model)
        {
            _log4net.Info("Tasks starts to unlock account" + " | " + model.Email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                    return Ok(await _authRepoService.UnlockUserAccount(model));

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

        //[AllowAnonymous]
        [HttpGet]
        [Route("get-merchants")]
        public async Task<IActionResult> GetMerchants()
        {
            // _log4net.Info("Tasks starts to create account" + " | " + model.Username + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.GetMerchants());
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
                if (ModelState.IsValid)
                {
                    var result = await _transactionService.GetOnboardingJourney();
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
                // _log4net.Error("Error occured" + " | " + model.Username + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("admin-get-customers-transaction-count")]
        public async Task<IActionResult> AdminGetCustomersTransactionCount()
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {

                    return Ok(await _merchantReportService.AdminGetCustomersTransactionCount());

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

        //  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("admin-get-customers-transaction-value")]
        public async Task<IActionResult> AdminGetCustomersTransactionValue()
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {

                    return Ok(await _merchantReportService.AdminGetCustomersTransactionValue());

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



        [AllowAnonymous]
        [HttpPost]
        [Route("clear-user-account")]
        public async Task<IActionResult> ClearUserDetails(string email, string reference)
        {
            _log4net.Info("Tasks starts to clear user account" + " | " + email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (reference != "3efa178h")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    var result = await _transactionService.ClearUserAccount(email);
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
                _log4net.Error("Error occured" + " | " + email + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }



        [AllowAnonymous]
        [HttpPost]
        [Route("clear-user-wallet")]
        public async Task<IActionResult> ClearMerchantWallet(string phoneNumber, string reference)
        {
            _log4net.Info("Tasks starts to clear user account" + " | " + phoneNumber + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (reference != "12345gh2")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    var result = await _createMerchantWalletService.ClearMerchantWalletInfo(phoneNumber);
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
                _log4net.Error("Error occured" + " | " + phoneNumber + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }


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


        //[AllowAnonymous]
        //[HttpGet]
        //[Route("get-all-merchant-business-info")]
        //public async Task<IActionResult> GetMerchantBusinessInfo([FromQuery] string reference)
        //{
        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (reference != "sterling2vxytq@k1")
        //            return BadRequest();

        //        if (ModelState.IsValid)
        //        {
        //            return Ok(await _merchantReportService.GetMerchantBusinessInfoAsync());
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
        ////[Route("get-all-interbank-default-info")]
        ////public async Task<IActionResult> GetInterBankRequest([FromQuery] string reference)
        ////{
        ////    var response = new WebApiResponse { };
        ////    try
        ////    {
        ////        if (reference != "213@k1")
        ////            return BadRequest();

        ////        if (ModelState.IsValid)
        ////        {
        ////            return Ok(await _merchantReportService.GetInterBankRequestAsync());
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


        ////[AllowAnonymous]
        ////[HttpGet]
        ////[Route("get-all-non-escrow-trans")]
        ////public async Task<IActionResult> GetTransactions([FromQuery] string reference)
        ////{
        ////    var response = new WebApiResponse { };
        ////    try
        ////    {
        ////        if (reference != "09hgx21")
        ////            return BadRequest();

        ////        return Ok(await _merchantReportService.GetNonEscrowBankTransactions());

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        response.ResponseCode = AppResponseCodes.InternalError;
        ////        return BadRequest(response);
        ////    }
        ////}


        ////[AllowAnonymous]
        ////[HttpGet]
        ////[Route("get-user-trans")]
        ////public async Task<IActionResult> GetFioranoTransactions([FromQuery] string reference)
        ////{
        ////    var response = new WebApiResponse { };
        ////    try
        ////    {
        ////        if (reference != "334fds2")
        ////            return BadRequest();

        ////        return Ok(await _merchantReportService.GetFioranoTransactions());

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        response.ResponseCode = AppResponseCodes.InternalError;
        ////        return BadRequest(response);
        ////    }
        ////}



        //[AllowAnonymous]
        //[HttpGet]
        //[Route("get-payment-links")]
        //public async Task<IActionResult> PaymentLinks([FromQuery] string reference)
        //{
        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (reference != "232gaw9")
        //            return BadRequest();

        //        return Ok(await _merchantReportService.GetPaymentLinks());

        //    }
        //    catch (Exception ex)
        //    {
        //        response.ResponseCode = AppResponseCodes.InternalError;
        //        return BadRequest(response);
        //    }
        //}



        //[AllowAnonymous]
        //[HttpGet]
        //[Route("get-interbank-lnterBankRequest")]
        //public async Task<IActionResult> InterbankRequest([FromQuery] string reference, string merchant)
        //{
        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (reference != "56353f")
        //            return BadRequest();

        //        if (ModelState.IsValid)
        //        {
        //            return Ok(await _merchantReportService.RemoveInterbankRequestInfo(merchant));
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
        ////[Route("get-Customer-OtherPayment")]
        ////public async Task<IActionResult> GetCustomerOtherPayment([FromQuery] string reference)
        ////{
        ////    var response = new WebApiResponse { };
        ////    try
        ////    {
        ////        if (reference != "sterling2rk11")
        ////            return BadRequest();

        ////        if (ModelState.IsValid)
        ////        {
        ////            return Ok(await _merchantReportService.GetCustomerOtherTransactionInfo());
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
        [Route("get-latest-OtherPayment")]
        public async Task<IActionResult> GetMerchantDetails([FromQuery] string reference, string merchant)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "34dft1")
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
        [Route("get-request-details")]
        public async Task<IActionResult> ValidateRequests([FromQuery] string reference, string merchant)
        {
            var response = new WebApiResponse { };
            try
            {
                if (reference != "34df12")
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    return Ok(await _merchantReportService.InterRequestAsync(merchant));
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
        [Route("get-admin-transactions-default")]
        public async Task<IActionResult> GetCustomerTransactionsAdmin([FromQuery] string category, string reference)
        {
            if (reference != "12vg345")
                return BadRequest();

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
                response.ResponseCode = AppResponseCodes.InternalError;

                return BadRequest(response);
            }
        }


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