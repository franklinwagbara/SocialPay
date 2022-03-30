using Microsoft.Extensions.Options;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.Merchant;
using System;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Merchant
{
    public class MerchantBusinessInfoBaseService
    {
        private readonly IMerchantBusinessInfoService _merchantBusinessInfoService;
        private readonly AppSettings _appSettings;
        private readonly MerchantsLogger _merchantLogger;
        public MerchantBusinessInfoBaseService(IMerchantBusinessInfoService merchantBusinessInfoService,
            IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _merchantBusinessInfoService = merchantBusinessInfoService ?? throw new ArgumentNullException(nameof(merchantBusinessInfoService));
        }

        public async Task<WebApiResponse> GetMerchantBusinessInfoAsync(long clientId)
        {
            //clientId = 179;
            try
            {
                var request = await _merchantBusinessInfoService.GetMerchantBusinessInfo(clientId);

                if (request == default)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record not found or update your business info", Message= "Record not found or update your business info" };
                
                request.Logo = request.Logo == string.Empty ? string.Empty : _appSettings.BaseApiUrl + request.FileLocation + "/" + request.Logo;

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
            }
            catch (Exception ex)
            {
                _merchantLogger.LogRequest($"{"GetMerchantBusinessInfoAsync"}{ ex.Message.ToString() }{" | "}{DateTime.Now}");
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured" };
            }
        }
    }
}
