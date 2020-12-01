using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Account;
using SocialPay.Core.Services.Authentication;
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
        private readonly CreateMerchantWalletService _createMerchantWalletService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AdminController));

        public AdminController(ADRepoService aDRepoService, MerchantReportService merchantReportService,
            TransactionService transactionService, AuthRepoService authRepoService,
            CreateMerchantWalletService createMerchantWalletService)
        {
            _aDRepoService = aDRepoService;
            _merchantReportService = merchantReportService;
            _transactionService = transactionService;
            _authRepoService = authRepoService;
            _createMerchantWalletService = createMerchantWalletService;
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



        [HttpPost]
        [Route("unlock-user-account")]
        public async Task<IActionResult> UnLockAccount([FromBody] UpdateUserRequestDto model)
        {
            _log4net.Info("Tasks starts to unlock account" + " | " + model.Email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _authRepoService.UnlockUserAccount(model);
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
                    var result = await _merchantReportService.GetMerchants();
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
                    var result = await _transactionService.GetCustomerOrders(category);
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
        [AllowAnonymous]
        [HttpPost]
        [Route("clear-user-account")]
        public async Task<IActionResult> ClearUserDetails(string email)
        {
            _log4net.Info("Tasks starts to clear user account" + " | " + email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
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
        public async Task<IActionResult> ClearMerchantWallet(string phoneNumber)
        {
            _log4net.Info("Tasks starts to clear user account" + " | " + phoneNumber + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
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
    }
}