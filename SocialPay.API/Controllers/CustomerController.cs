using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Customer;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;

namespace SocialPay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerRepoService _customerRepoService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(CustomerController));

        public CustomerController(CustomerRepoService customerRepoService)
        {
            _customerRepoService = customerRepoService;
        }

        [HttpGet]
        [Route("payment-details")]
        public async Task<IActionResult> GetPaymentLinkDetails([FromQuery] CustomerRequestDto model)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                  
                    var result = await _customerRepoService.GetLinkDetails(model.TransactionReference);                  
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
        [Route("initiate-payment")]
        public async Task<IActionResult> InitiatePayment([FromForm] CustomerPaymentRequestDto model)
        {
            var response = new WebApiResponse { };
            _log4net.Info("Task starts to initiate payments" + " | " + model.TransactionReference + " | " + DateTime.Now);

            try
            {
                if (ModelState.IsValid)
                {

                    var result = await _customerRepoService.MakePayment(model);
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
                _log4net.Error("An error occured while initiating payment" + " | " + model.TransactionReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("payment-confirmation")]
        public async Task<IActionResult> ValidatePayment([FromBody] PaymentValidationRequestDto model)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {

                    var result = await _customerRepoService.PaymentConfirmation(model);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("accept-reject-order")]
        public async Task<IActionResult> AcceptRejectOrder([FromBody] AcceptRejectRequestDto model)
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
                    var result = await _customerRepoService.AcceptOrRejectItem(model, Convert.ToInt32(clientId));
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("get-orders")]
        public async Task<IActionResult> GetOrders()
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
                    var result = await _customerRepoService.GetAllCustomerOrders(Convert.ToInt32(clientId));
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