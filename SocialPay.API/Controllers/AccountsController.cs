using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Account;
using SocialPay.Core.Services.Authentication;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;

namespace SocialPay.API.Controllers
{
    [Route("api/socialpay")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly MerchantRegistrationService _merchantRegistrationService;
        private readonly AuthRepoService _authRepoService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AccountsController));

        public AccountsController(MerchantRegistrationService merchantRegistrationService,
            AuthRepoService authRepoService)
        {
            _merchantRegistrationService = merchantRegistrationService;
            _authRepoService = authRepoService;
        }

        [HttpPost]
        [Route("merchant-signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestDto model)
        {
            _log4net.Info("Tasks starts to create account" + " | " + model.Email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _merchantRegistrationService.CreateNewMerchant(model);                  
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

       // [AllowAnonymous]
        [HttpPost]
        [Route("signup-confirmation")]
        public async Task<IActionResult> SignUpConfirmation([FromBody] SignUpConfirmationRequestDto model)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _merchantRegistrationService.ConfirmSignUp(model);                  
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




        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> ValidateLogin([FromBody] LoginRequestDto model)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _authRepoService.Authenticate(model);                   
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