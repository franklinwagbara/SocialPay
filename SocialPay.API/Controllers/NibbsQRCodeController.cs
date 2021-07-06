using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.QrCode;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialPay.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/socialpay/qRcode")]
    [ApiController]
    public class NibbsQRCodeController : ControllerBase
    {
        private readonly NibbsQrBaseService _nibbsQrBaseService;
        public NibbsQRCodeController(NibbsQrBaseService nibbsQrBaseService)
        {
            _nibbsQrBaseService = nibbsQrBaseService ?? throw new ArgumentNullException(nameof(nibbsQrBaseService));

        }

        [HttpPost]
        [Route("create-QrCode-merchant")]
        public async Task<IActionResult> CreateUser([FromBody] CreateNibsMerchantRequestDto model)
        {
            // _log4net.Info("Tasks starts to create account" + " | " + model.Email + " | " + DateTime.Now);
            var response = new WebApiResponse { };

            if (ModelState.IsValid)
            {
                var identity = User.Identity as ClaimsIdentity;
                var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                return Ok(await _nibbsQrBaseService.CreateMerchantAsync(model, Convert.ToInt32(clientId)));
            }

            var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            response.ResponseCode = AppResponseCodes.Failed;
            response.Data = message;

            return BadRequest(response);
        }


        [HttpPost]
        [Route("confirm-QrCode-merchant")]
        public async Task<IActionResult> ConfirmMerchantOnboarding([FromBody] CreateNibbsSubMerchantDto model)
        {
            // _log4net.Info("Tasks starts to create account" + " | " + model.Email + " | " + DateTime.Now);
            var response = new WebApiResponse { };

            if (ModelState.IsValid)
            {
                var identity = User.Identity as ClaimsIdentity;
                var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                return Ok(await _nibbsQrBaseService.CreateSubMerchantAsync(model, Convert.ToInt32(clientId)));
            }

            var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            response.ResponseCode = AppResponseCodes.Failed;
            response.Data = message;

            return BadRequest(response);
        }


        [HttpPost]
        [Route("bind-merchant")]
        public async Task<IActionResult> BindMerchant([FromBody] BindMerchantRequestDto model)
        {
            // _log4net.Info("Tasks starts to create account" + " | " + model.Email + " | " + DateTime.Now);
            var response = new WebApiResponse { };

            if (ModelState.IsValid)
            {
                var identity = User.Identity as ClaimsIdentity;
                var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                return Ok(await _nibbsQrBaseService.BindMerchantAsync(model, Convert.ToInt32(clientId)));
            }

            var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            response.ResponseCode = AppResponseCodes.Failed;
            response.Data = message;

            return BadRequest(response);
        }
    }
}
