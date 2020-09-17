using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> InitiatePayment([FromBody] CustomerPaymentRequestDto model)
        {
            var response = new WebApiResponse { };
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
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("validate-payment")]
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

        [HttpPost]
        [Route("accept-reject-order")]
        public async Task<IActionResult> AcceptRejectOrder([FromBody] CustomerPaymentRequestDto model)
        {
            var response = new WebApiResponse { };
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
                response.ResponseCode = AppResponseCodes.InternalError;
                return BadRequest(response);
            }
        }
    }
}