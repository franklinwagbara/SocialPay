using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Services.Tenant;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Super Administrator")]
    public class TenantsController : BaseController
    {
        private readonly TenantProfileAPIService _tenantProfileAPIService;
        public TenantsController(TenantProfileAPIService tenantProfileAPIService, INotification notification) : base(notification)
        {
            _tenantProfileAPIService = tenantProfileAPIService ?? throw new ArgumentNullException(nameof(tenantProfileAPIService));
        }

        [HttpPost]
        [Route("create-tenant-profile")]
        public async Task<IActionResult> CreateTenantProfile([FromBody] TenantProfileRequestDto request) => Response(await _tenantProfileAPIService.CreateNewTenant(request, User.GetSessionDetails().ClientId).ConfigureAwait(false));

        [HttpGet]
        [Route("get-tenant")]
        public async Task<IActionResult> GetTenant() => Response(await _tenantProfileAPIService.GetTenant(User.GetSessionDetails().ClientId).ConfigureAwait(false));
    }
}
