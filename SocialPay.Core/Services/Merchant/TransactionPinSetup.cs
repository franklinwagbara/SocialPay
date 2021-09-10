using Microsoft.Extensions.Options;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SocialPay.Core.Extensions.Common;

namespace SocialPay.Core.Services.Merchant
{
    public class TransactionPinSetup
    {
        private readonly IMerchantTransactionSetup _merchantTransactionSetup;
        private readonly AppSettings _appSettings;
        public TransactionPinSetup(IMerchantTransactionSetup merchantTransactionSetup,
            IOptions<AppSettings> appSettings)
        {
            _merchantTransactionSetup = merchantTransactionSetup ?? throw new ArgumentNullException(nameof(merchantTransactionSetup));
            _appSettings = appSettings.Value;
        }

        public async Task<WebApiResponse> TransactionPinSetupAsync(long clientId, string pin)
        {
            try
            {
               // clientId = 203;

                if (await _merchantTransactionSetup.ExistsAsync(clientId))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicatePinSetup, Message = "Transaction PIN exist" };

                var securePin = pin.Encrypt(_appSettings.MerchantSetupKey);

                var model = new MerchantTransactionSetupViewModel
                {
                    ClientAuthenticationId = clientId,
                    Status = true,
                    Pin = securePin,
                    LastDateModified = DateTime.Now
                };

                await _merchantTransactionSetup.AddAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Transaction PIN was successfully setup" };

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured while setting transaction PIN" };
            }
        }


        public async Task<WebApiResponse> ValidateTransactionAsync(long clientId, string pin)
        {
            try
            {
                //clientId = 203;

                var merchantInfo = await _merchantTransactionSetup.GetMerchantValidationInfo(clientId, pin.Encrypt(_appSettings.MerchantSetupKey));
                
                if (merchantInfo == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found/Invalid Pin" };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Vlidation was successful" };

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured while setting transaction PIN" };
            }
        }

    }
}
