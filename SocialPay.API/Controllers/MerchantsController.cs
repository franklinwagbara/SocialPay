using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Repositories.Invoice;
using SocialPay.Core.Services.Account;
using SocialPay.Core.Services.Report;
using SocialPay.Core.Services.Specta;
using SocialPay.Core.Services.Transaction;
using SocialPay.Core.Services.Wallet;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;

namespace SocialPay.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Merchant")]
    [Route("api/socialpay/merchant")]
    [ApiController]
    public class MerchantsController : ControllerBase
    {
        private readonly MerchantRegistrationService _merchantRegistrationService;
        private readonly MerchantPaymentLinkService _merchantPaymentLinkService;
        private readonly MerchantReportService _merchantReportService;
        private readonly InvoiceService _invoiceService;
        private readonly CreateMerchantWalletService _createMerchantWalletService;
        private readonly DisputeRepoService _disputeRepoService;
        private readonly PayWithSpectaService _payWithSpectaService;
        public MerchantsController(MerchantRegistrationService merchantRegistrationService,
            MerchantPaymentLinkService merchantPaymentLinkService, MerchantReportService merchantReportService,
            InvoiceService invoiceService, CreateMerchantWalletService createMerchantWalletService,
            DisputeRepoService disputeRepoService, PayWithSpectaService payWithSpectaService)
        {
            _merchantRegistrationService = merchantRegistrationService;
            _merchantPaymentLinkService = merchantPaymentLinkService;
            _merchantReportService = merchantReportService;
            _invoiceService = invoiceService;
            _createMerchantWalletService = createMerchantWalletService;
            _disputeRepoService = disputeRepoService;
            _payWithSpectaService = payWithSpectaService;
        }

        [HttpPost]
        [Route("onboarding-business-info")]
        public async Task<IActionResult> MerchantBusinessInfo([FromForm] MerchantOnboardingInfoRequestDto model)
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
                    var result = await _merchantRegistrationService.OnboardMerchantBusinessInfo(model, Convert.ToInt32(clientId));
                    //if (result.ResponseCode != AppResponseCodes.Success)
                    //    return BadRequest(result);
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

        //[AllowAnonymous]
        [HttpPost]
        [Route("onboarding-bank-info")]
        public async Task<IActionResult> MerchantBankInfo([FromBody] MerchantBankInfoRequestDto model)
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
                    var result = await _merchantRegistrationService.OnboardMerchantBankInfo(model, Convert.ToInt32(clientId));
                    //if (result.ResponseCode != AppResponseCodes.Success)
                    //    return BadRequest(result);
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

       // [AllowAnonymous]
        [HttpGet]
        [Route("list-of-banks")]
        public async Task<IActionResult> GetBanks()
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                   
                    var result = await _merchantRegistrationService.GetListOfBanks();
          
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

       // [AllowAnonymous]
        [HttpPost]
        [Route("transaction-setup")]
        public async Task<IActionResult> TransactionSetup([FromBody] MerchantActivitySetupRequestDto model)
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
                    var result = await _merchantRegistrationService.TransactionSetupRequest(model, Convert.ToInt32(clientId));
                    //if (result.ResponseCode != AppResponseCodes.Success)
                    //    return BadRequest(result);
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


       // [AllowAnonymous]
        [HttpPost]
        [Route("create-merchant-wallet")]
        public async Task<IActionResult> CreateWallet([FromBody] WalletRequestDto model)
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
                    var result = await _createMerchantWalletService.CreateWallet(Convert.ToInt32(clientId));
                    //if (result.ResponseCode != AppResponseCodes.Success)
                    //    return BadRequest(result);
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



        // [AllowAnonymous]
        [HttpGet]
        [Route("payment-links")]
        public async Task<IActionResult> GetPaymentDetails()
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
                    var result = await _merchantPaymentLinkService.GetAllPaymentLinksByMerchant(Convert.ToInt32(clientId));
                    //if (result.ResponseCode != AppResponseCodes.Success)
                    //    return BadRequest(result);
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

       // [AllowAnonymous]
        [HttpGet]
        [Route("customers")]
        public async Task<IActionResult> GetMerchantCustomers()
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
                    var result = await _merchantPaymentLinkService.GetCustomers(Convert.ToInt32(clientId));
                  
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

        //[AllowAnonymous]
        [HttpPost]
        [Route("send-receipt")]
        public async Task<IActionResult> SendCustomerReceipt([FromBody] CustomerReceiptRequestDto model)
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
                    var result = await _merchantReportService.GenerateCustomerReceipt(model);
                    //if (result.ResponseCode != AppResponseCodes.Success)
                    //    return BadRequest(result);
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

       // [AllowAnonymous]
        [HttpPost]
        [Route("send-Invoice")]
        public async Task<IActionResult> SendInvoice([FromBody] InvoiceRequestDto model)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var identity = User.Identity as ClaimsIdentity;
                    var clientName = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    var businessName = identity.Claims.FirstOrDefault(c => c.Type == "businessName")?.Value;
                    var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    var result = await _merchantPaymentLinkService
                        .GenerateInvoice(model, Convert.ToInt32(clientId), businessName);
                    //if (result.ResponseCode != AppResponseCodes.Success)
                    //    return BadRequest(result);
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


        //[HttpPost]
        //[Route("log-dispute")]
        //public async Task<IActionResult> LogDispute([FromBody] DisputeItemRequestDto model)
        //{
        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var identity = User.Identity as ClaimsIdentity;
        //            var clientName = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        //            var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        //            var businessName = identity.Claims.FirstOrDefault(c => c.Type == "businessName")?.Value;
        //            var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        //            var result = await _disputeRepoService.LogDisputeRequest(model, Convert.ToInt32(clientId));
                  
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

        // [AllowAnonymous]
        [HttpGet]
        [Route("get-invoice")]
        public async Task<IActionResult> GetMerchantInvoices()
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
                    var result = await _merchantReportService.GetAllInvoiceByMerchantId(Convert.ToInt32(clientId));

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


        // [AllowAnonymous]
        [HttpGet]
        [Route("get-escrow-transactions")]
        public async Task<IActionResult> GetEscrows([FromQuery] string status)
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
                    var result = await _merchantReportService
                        .GetAllEscrowTransactions(Convert.ToInt32(clientId), status);
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
        //[AllowAnonymous]
        [HttpGet]
        [Route("invoice-payment-details")]
        public async Task<IActionResult> GetInvoiceTransaction()
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
                    var result = await _invoiceService.GetInvoiceTransactionDetails(Convert.ToInt32(clientId));
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


        //[AllowAnonymous]
        //[HttpGet]
        //[Route("insert-login-status")]
        //public async Task<IActionResult> InsertLoginStatus()
        //{
        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {

        //            var result = await _merchantReportService.InsertData();
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

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("verify-pay-with-specta")]
        //public async Task<IActionResult> VerifyPayWithSpecta(string verifyPayment)
        //{
        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {

        //            var result = await _payWithSpectaService.PaymentVerification(verifyPayment);
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

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("get-cache")]
        //public async Task<IActionResult> CacheTest()
        //{
        //    var response = new WebApiResponse { };
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {

        //            var result = await _merchantReportService.RedisCacheTest();
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