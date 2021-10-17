using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Repositories.Invoice;
using SocialPay.Core.Services.Customer;
using SocialPay.Core.Services.Store;
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
        private readonly StoreBaseRepository _storeBaseRepository;
        private readonly InvoiceService _invoiceService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(CustomerController));

        public CustomerController(CustomerRepoService customerRepoService,
            InvoiceService invoiceService, StoreBaseRepository storeBaseRepository)
        {
            _customerRepoService = customerRepoService;
            _invoiceService = invoiceService;
            _storeBaseRepository = storeBaseRepository ?? throw new ArgumentNullException(nameof(storeBaseRepository));
        }

        [HttpGet]
        [Route("payment-details")]
        public async Task<IActionResult> GetPaymentLinkDetails([FromQuery] CustomerRequestDto model)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)                  
                    return Ok(await _customerRepoService.GetLinkDetails(model.TransactionReference));

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
        [Route("initiate-payment")]
        public async Task<IActionResult> InitiatePayment([FromForm] CustomerPaymentRequestDto model)
        {
            var response = new WebApiResponse { };
            _log4net.Info("Task starts to initiate payments" + " | " + model.TransactionReference + " | " + DateTime.Now);

            try
            {             

                if (ModelState.IsValid)
                    return Ok(await _customerRepoService.InitiatePayment(model));

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

                return StatusCode(500, response);
            }
        }


        [HttpPost]
        [Route("initiate-store-payment")]
        public async Task<IActionResult> InitiateStorePayment([FromBody] CustomerStorePaymentRequestDto model)
        {
            var response = new WebApiResponse { };
            _log4net.Info("Task starts to initiate store payments" + " | " + model.TransactionReference + " | " + DateTime.Now);

            try
            {

                if (ModelState.IsValid)
                    return Ok(await _storeBaseRepository.InitiatePayment(model));

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

                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route("initiate-test")]
        public async Task<IActionResult> Test([FromBody] TestVideoDto model)
        {
            var response = new WebApiResponse { };

            try
            {
                return Ok(model);             

            }
            catch (Exception)
            {
                response.ResponseCode = AppResponseCodes.InternalError;

                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route("payment-confirmation")]
        public async Task<IActionResult> PaymentConfirmation([FromBody] PaymentValidationRequestDto model)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                    return Ok(await _customerRepoService.PaymentConfirmation(model));

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
        [Route("ussd-payment-confirmation")]
        public async Task<IActionResult> UssdPaymentConfirmation([FromBody] PaymentValidationRequestDto model)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                    return Ok(await _customerRepoService.PaymentConfirmation(model));

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
        [HttpGet]
        [Route("get-orders")]
        public async Task<IActionResult> GetOrders([FromQuery] string category)
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
                 
                    return Ok(await _customerRepoService
                        .GetAllCustomerOrders(Convert.ToInt32(clientId), category));
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


        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpGet]
        //[Route("get-orders")]
        //public async Task<IActionResult> RejectOrder([FromQuery] string category)
        //{
        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var identity = User.Identity as ClaimsIdentity;
        //            var clientName = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        //            var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        //            var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        //            var result = await _customerRepoService
        //                .GetAllCustomerOrders(Convert.ToInt32(clientId), category);
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
        //        response.ResponseCode = AppResponseCodes.InternalError;
        //        return BadRequest(response);
        //    }
        //}

    }
}