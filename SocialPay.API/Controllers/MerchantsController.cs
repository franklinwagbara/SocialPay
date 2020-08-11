using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Account;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;

namespace SocialPay.API.Controllers
{
    [Route("api/socialpay")]
    [ApiController]
    public class MerchantsController : ControllerBase
    {
        private readonly MerchantRegistrationService _merchantRegistrationService;
        public MerchantsController(MerchantRegistrationService merchantRegistrationService)
        {
            _merchantRegistrationService = merchantRegistrationService;
        }

        [HttpPost]
        [Route("merchant-onboarding")]
        public async Task<IActionResult> ValidateLogin([FromForm] MerchantOnboardingRequestDto model)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _merchantRegistrationService.OnboardMerchant(model);
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