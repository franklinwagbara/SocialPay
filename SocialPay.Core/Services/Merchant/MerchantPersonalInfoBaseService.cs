using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Merchant
{
    public class MerchantPersonalInfoBaseService
    {
        private readonly IPersonalInfoService _personalInfoService;

        public MerchantPersonalInfoBaseService(IPersonalInfoService personalInfoService)
        {
            _personalInfoService = personalInfoService ?? throw new ArgumentNullException(nameof(personalInfoService));
        }

        public async Task<WebApiResponse> GetMerchantPersonalInfoAsync(long clientId)
        {
           // clientId = 90;
            try
            {
                var request = await _personalInfoService.GetMerchantPersonalInfo(clientId);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured" };
            }
        }
    }
}
