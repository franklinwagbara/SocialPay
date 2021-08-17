using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Tenant
{
    public class TenantProfileAPIService
    {
        private readonly ITenantProfileService _tenantProfileService;
        public TenantProfileAPIService(ITenantProfileService tenantProfileService)
        {
            _tenantProfileService = tenantProfileService ?? throw new ArgumentNullException(nameof(tenantProfileService));
        }

        public async Task<WebApiResponse> CreateNewTenant(TenantProfileRequestDto request, long clientId)
        {
            try
            {
                if (await _tenantProfileService.ExistsByEmailAsync(request.Email.ToLower(), request.PhoneNumber))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails, Message = "Duplicate email or phone number" };

                var model = new TenantProfileViewModel
                {
                    Address = request.Address,
                    Email = request.Email.ToLower(),
                    PhoneNumber = request.PhoneNumber,
                    TenantName = request.TenantName,
                    WebSiteUrl = request.WebSiteUrl,
                    ClientAuthenticationId = clientId
                };

                await _tenantProfileService.AddAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Tenant was successfully created" };

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Internal error occured" };
            }
        }


        public async Task<WebApiResponse> GetTenant(long clientId)
        {
            try
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = await _tenantProfileService.GetAllAsync() };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured" };
            }
        }
    }
}
