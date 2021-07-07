using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialPay.API.Controllers
{
    [Route("api/socialpay/transaction")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        public BillsController()
        {

        }


        [HttpPost]
        [Route("dstv-subscription")]
        public async Task<IActionResult> CreateUser([FromBody] string Number)
        {
            // _log4net.Info("Tasks starts to create account" + " | " + model.Email + " | " + DateTime.Now);
            var response = new WebApiResponse { };

            if (ModelState.IsValid)
            {
                var identity = User.Identity as ClaimsIdentity;
                var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                return Ok();
            }

            var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            response.ResponseCode = AppResponseCodes.Failed;
            response.Data = message;

            return BadRequest(response);
        }
    }
}
