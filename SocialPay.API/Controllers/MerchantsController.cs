using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Account;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;

namespace SocialPay.API.Controllers
{
    [Authorize]
    [Route("api/socialpay/merchant")]
    [ApiController]
    public class MerchantsController : ControllerBase
    {
        private readonly MerchantRegistrationService _merchantRegistrationService;
        public MerchantsController(MerchantRegistrationService merchantRegistrationService)
        {
            _merchantRegistrationService = merchantRegistrationService;
        }

        [HttpPost]
        [Route("onboarding-business-info")]
        public async Task<IActionResult> ValidateLogin([FromForm] MerchantOnboardingInfoRequestDto model)
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
                    var result = await _merchantRegistrationService.OnboardMerchant(model, Convert.ToInt32(clientId));
                    if (result.ResponseCode != AppResponseCodes.Success)
                        return BadRequest(result);
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