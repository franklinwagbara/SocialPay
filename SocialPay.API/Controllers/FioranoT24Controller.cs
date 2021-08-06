using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Fiorano;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialPay.Core.Extensions.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace SocialPay.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/socialpay/fiorano")]
    [ApiController]
    public class FioranoT24Controller : BaseController
    {
        private readonly FioranoService _fioranoService;
        public FioranoT24Controller(FioranoService fioranoService, INotification notification) : base(notification)
        {
            _fioranoService = fioranoService ?? throw new ArgumentNullException(nameof(fioranoService));
        }

        [HttpPost]
        [Route("debit-customer-account")]
        public async Task<IActionResult> DebitCustomerAccount([FromBody] FioranoBillsRequestDto request) => Response(await _fioranoService.InitiateFioranoRequest(request, User.GetSessionDetails().ClientId).ConfigureAwait(false));

    }
}
