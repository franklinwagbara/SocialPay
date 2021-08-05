using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Services.QrCode;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.Notification;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialPay.API.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/socialpay/qRcode")]
    [ApiController]
    public class NibbsQRCodeController : BaseController
    {
        private readonly NibbsQrBaseService _nibbsQrBaseService;
        public NibbsQRCodeController(NibbsQrBaseService nibbsQrBaseService, INotification notification) : base(notification)
        {
            _nibbsQrBaseService = nibbsQrBaseService ?? throw new ArgumentNullException(nameof(nibbsQrBaseService));

        }

        ////[HttpPost]
        ////[Route("create-merchant")]
        ////public async Task<IActionResult> CreateQrMerchant([FromBody] DefaultMerchantRequestDto model)
        ////{
        ////    // _log4net.Info("Tasks starts to create account" + " | " + model.Email + " | " + DateTime.Now);
        ////    var response = new WebApiResponse { };

        ////    if (ModelState.IsValid)
        ////    {
        ////        var identity = User.Identity as ClaimsIdentity;
        ////        var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        ////        return Ok(await _nibbsQrBaseService.CreateMerchantAsync(model, Convert.ToInt32(clientId)));
        ////    }

        ////    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        ////        .Select(e => e.ErrorMessage));

        ////    response.ResponseCode = AppResponseCodes.Failed;
        ////    response.Data = message;

        ////    return BadRequest(response);
        ////}


        ////[HttpPost]
        ////[Route("create-sub-merchant")]
        ////public async Task<IActionResult> CreateSubMerchantOnboarding([FromBody] DefaultSubMerchantRequestDto model)
        ////{
        ////    // _log4net.Info("Tasks starts to create account" + " | " + model.Email + " | " + DateTime.Now);
        ////    var response = new WebApiResponse { };

        ////    if (ModelState.IsValid)
        ////    {
        ////        var identity = User.Identity as ClaimsIdentity;
        ////        var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        ////        return Ok(await _nibbsQrBaseService.CreateSubMerchantAsync(model, Convert.ToInt32(clientId)));
        ////    }

        ////    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        ////        .Select(e => e.ErrorMessage));

        ////    response.ResponseCode = AppResponseCodes.Failed;
        ////    response.Data = message;

        ////    return BadRequest(response);
        ////}


        ////[HttpPost]
        ////[Route("bind-merchant")]
        ////public async Task<IActionResult> BindMerchant()
        ////{
        ////    // _log4net.Info("Tasks starts to create account" + " | " + model.Email + " | " + DateTime.Now);
        ////    var response = new WebApiResponse { };

        ////    if (ModelState.IsValid)
        ////    {
        ////        var identity = User.Identity as ClaimsIdentity;
        ////        var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        ////        return Ok(await _nibbsQrBaseService.BindMerchantAsync(Convert.ToInt32(clientId)));
        ////    }

        ////    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        ////        .Select(e => e.ErrorMessage));

        ////    response.ResponseCode = AppResponseCodes.Failed;
        ////    response.Data = message;

        ////    return BadRequest(response);
        ////}


        [HttpGet]
        [Route("get-web-hook-filter")]
        public async Task<IActionResult> GetWebHook() => Response(await _nibbsQrBaseService.WebHookFilterAsync(User.GetSessionDetails().ClientId).ConfigureAwait(false));


        [HttpPost]
        [Route("register-webhook")]
        public async Task<IActionResult> RegisterWebHook([FromBody] RegisterWebhookRequestDto request) => Response(await _nibbsQrBaseService.RegsiterWebHook(request, User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [HttpPost]
        [Route("pay-with-QR")]
        public async Task<IActionResult> PayWithQrCode([FromBody] DynamicPaymentRequestDto request) => Response(await _nibbsQrBaseService.DynamicPaymentAsync(request, User.GetSessionDetails().ClientId).ConfigureAwait(false));
        //public async Task<IActionResult> PayWithQrCode([FromBody] DynamicPaymentRequestDto model)
        //{
        //    // _log4net.Info("Tasks starts to create account" + " | " + model.Email + " | " + DateTime.Now);
        //    var response = new WebApiResponse { };

        //    if (ModelState.IsValid)
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        //        return Ok(await _nibbsQrBaseService.DynamicPaymentAsync(model, Convert.ToInt32(clientId)));
        //    }

        //    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
        //        .Select(e => e.ErrorMessage));

        //    response.ResponseCode = AppResponseCodes.Failed;
        //    response.Data = message;

        //    return BadRequest(response);
        //}
    }
}
