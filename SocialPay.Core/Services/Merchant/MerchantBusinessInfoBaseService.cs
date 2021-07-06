using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Merchant
{
    public class MerchantBusinessInfoBaseService
    {
        private readonly IMerchantBusinessInfoService _merchantBusinessInfoService;

        public MerchantBusinessInfoBaseService(IMerchantBusinessInfoService merchantBusinessInfoService)
        {
            _merchantBusinessInfoService = merchantBusinessInfoService ?? throw new ArgumentNullException(nameof(merchantBusinessInfoService));
        }

        public async Task<WebApiResponse> GetMerchantBusinessInfoAsync(long clientId)
        {
           // clientId = 90;
            try
            {
                var request = await _merchantBusinessInfoService.GetMerchantBusinessInfo(clientId);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured" };
            }
        }
    }
}
