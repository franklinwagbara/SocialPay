using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Customer;
using SocialPay.Core.Services.Report;
using SocialPay.Core.Services.Specta;
using SocialPay.Core.Services.Transaction;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;

namespace SocialPay.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/socialpay/transaction")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly MerchantPaymentLinkService _merchantPaymentLinkService;
        private readonly CustomerRepoService _customerRepoService;
        private readonly MerchantReportService _merchantReportService;
        private readonly DisputeRepoService _disputeRepoService;
        private readonly PayWithSpectaService _payWithSpectaService;

        public TransactionsController(MerchantPaymentLinkService merchantPaymentLinkService,
            CustomerRepoService customerRepoService, MerchantReportService merchantReportService,
            DisputeRepoService disputeRepoService,
            PayWithSpectaService payWithSpectaService
            )
        {
            _merchantPaymentLinkService = merchantPaymentLinkService;
            _customerRepoService = customerRepoService;
            _merchantReportService = merchantReportService;
            _disputeRepoService = disputeRepoService;
            _payWithSpectaService = payWithSpectaService;
        }

       // [AllowAnonymous]
        [HttpPost]
        [Route("generate-payment-link")]
        public async Task<IActionResult> GeneratePaymentLink([FromForm] MerchantpaymentLinkRequestDto model)
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
                    var userStatus = identity.Claims.FirstOrDefault(c => c.Type == "UserStatus")?.Value;                
                    
                    return Ok(await _merchantPaymentLinkService
                        .GeneratePaymentLink(model, Convert.ToInt32(clientId)));
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


        [HttpPut]
        [Route("update-payment-link")]
        public async Task<IActionResult> UpdatePaymentLink(string paymentLinkName, [FromBody] UpdatePaymentDTO model)
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
                    var userStatus = identity.Claims.FirstOrDefault(c => c.Type == "UserStatus")?.Value;

                    return Ok(await _merchantPaymentLinkService.UpdatePaymentLink(Convert.ToInt32(clientId), model, paymentLinkName));
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
       
        // [AllowAnonymous]
        [HttpDelete]
        [Route("delete-payment-link")]
        public async Task<IActionResult> DeletePaymentLink([FromQuery] long paymentLinkId)
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
                    var userStatus = identity.Claims.FirstOrDefault(c => c.Type == "UserStatus")?.Value;

                    return Ok(await _merchantPaymentLinkService.DeletePaymentLink(Convert.ToInt32(clientId), paymentLinkId));
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
        [Route("validate-custom-url-name")]
        public async Task<IActionResult> ValidateCustomUrl([FromQuery] string customUrlName)
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

                    return Ok(await _merchantPaymentLinkService.ValidateUrlAsync(customUrlName));
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
        [Route("get-customer-payments")]
        public async Task<IActionResult> GetCustomerPayments()
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
                    
                    return Ok(await _merchantPaymentLinkService.GetCustomerPayments(Convert.ToInt32(clientId)));
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

                    return Ok(await _customerRepoService.AcceptOrRejectItem(model, Convert.ToInt32(clientId)));
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

        [HttpPost]
        [Route("log-dispute")]
        public async Task<IActionResult> LogDispute([FromForm] DisputeItemRequestDto model)
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

                    return Ok(await _disputeRepoService.LogDisputeRequest(model, Convert.ToInt32(clientId)));
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
        [Route("get-disputes")]
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

                    return Ok(await _merchantReportService.GetAllLoggedDisputes(Convert.ToInt32(clientId), false));
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
        [Route("decrypt-transaction")]
        public async Task<IActionResult> DecryptTransactions([FromQuery] string responseMessage)
        {
            var response = new WebApiResponse { };
            try
            {

                if (ModelState.IsValid)
                {
                    return Ok(await _customerRepoService.DecryptMessage(responseMessage));
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
        [Route("decrypt-specta-transaction")]
        public async Task<IActionResult> DecryptSpectaTransactions([FromQuery] string responseMessage)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    return Ok(await _customerRepoService.DecryptSpectaMessage(responseMessage));
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

    }
}