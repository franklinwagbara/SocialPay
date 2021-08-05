using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Bill;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SocialPay.Core.Extensions.Common;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SocialPay.API.Controllers
{
    [Route("api/socialpay/bills")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BillsController : BaseController
    {
        private readonly BillService _billservice;
        public BillsController(BillService billService, INotification notification) : base(notification)
        {
            _billservice = billService ?? throw new ArgumentNullException(nameof(billService));
        }

        [HttpGet]
        [Route("dstv-gotv-get-bouquets")]
        public async Task<IActionResult> GetBillers() => Response(await _billservice.GetBillersAsync(User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [HttpPost]
        [Route("dstv-gotv-account-lookup")]
        public async Task<IActionResult> DstvGotvAccountLookp([FromBody] string customerId) => Response(await _billservice.PayUAccountLookupPayment(customerId, User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [HttpPost]
        [Route("dstv-gotv-single-payment")]
        public async Task<IActionResult> DstvGotvAccountSinglePayment([FromBody] SingleDstvPaymentDto request) => Response(await _billservice.PayUSingleDstvPayment(request, User.GetSessionDetails().ClientId).ConfigureAwait(false));
    }
}
