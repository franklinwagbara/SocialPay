using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Bill;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Notification;
using System;
using System.Threading.Tasks;
using SocialPay.Core.Extensions.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SocialPay.API.Controllers
{
    [Route("api/socialpay/bills")]
    [ApiController]
   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpGet]
        [Route("dstv-gotv-account-lookup")]
        public async Task<IActionResult> DstvGotvAccountLookp([FromQuery] AccountLookUpRequest request) => Response(await _billservice.PayUAccountLookupPayment(request.CustomerId, User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [HttpPost]
        [Route("dstv-gotv-single-payment")]
        public async Task<IActionResult> DstvGotvAccountSinglePayment([FromBody] SingleDstvPaymentDto request) => Response(await _billservice.PayUSingleDstvPayment(request, User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [HttpGet]
        [Route("network-providers")]
        public async Task<IActionResult> NetWorkProviders() => Response(await _billservice.GetNetworkProviders(User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [HttpGet]
        [Route("network-products")]
        public async Task<IActionResult> NetworkProducts([FromQuery] int billerId) => Response(await _billservice.GetAirtimeProducts(User.GetSessionDetails().ClientId, billerId).ConfigureAwait(false));

        //[HttpPost]
        //[Route("airtime-subscription}")]
        //public async Task<IActionResult> AirtimeSubscriptionRequest([FromBody] VendAirtimeDTO request) => Response(await _billservice.VendAirtimeRequest(request, User.GetSessionDetails().ClientId).ConfigureAwait(false));
    }
}
