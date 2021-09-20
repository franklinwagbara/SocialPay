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
using SocialPay.Core.Services.EventLogs;
using SocialPay.Helper.Dto.Request;

namespace SocialPay.Core.Services.Merchant
{
    public class TransactionPinSetup
    {
        private readonly IMerchantTransactionSetup _merchantTransactionSetup;
        private readonly AppSettings _appSettings;
        private readonly EventLogService _eventLogService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(TransactionPinSetup));

        public TransactionPinSetup(IMerchantTransactionSetup merchantTransactionSetup,
            IOptions<AppSettings> appSettings, EventLogService eventLogService)
        {
            _merchantTransactionSetup = merchantTransactionSetup ?? throw new ArgumentNullException(nameof(merchantTransactionSetup));
            _appSettings = appSettings.Value;
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
        }

        public async Task<WebApiResponse> TransactionPinSetupAsync(long clientId, string pin, string email)
        {
            _log4net.Info("Initiating transaction pin setup" + " | " + clientId + " | " + email + " - "+ DateTime.Now);

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

                var eventLog = new EventRequestDto
                {
                    ModuleAccessed = EventLogProcess.MerchantSignUp,
                    Description = "Transaction Pin was successfully setup",
                    UserId = email,
                    ClientAuthenticationId = clientId
                };

                await _eventLogService.ActivityRequestLog(eventLog);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Transaction PIN was successfully setup" };

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured to setup transaction PIN" + " | " + email + " | " + ex + " - " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured while setting transaction PIN" };
            }
        }


        public async Task<WebApiResponse> ValidateTransactionAsync(long clientId, string pin, string email)
        {
            _log4net.Info("validate transaction PIN" + " | " + email + " | " + DateTime.Now);

            try
            {
                //clientId = 203;

                if (!await _merchantTransactionSetup.ExistsAsync(clientId))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionPinDoesNotExit, Message = "Transaction pin not profiled/setup" };

                var merchantInfo = await _merchantTransactionSetup.GetMerchantValidationInfo(clientId, pin.Encrypt(_appSettings.MerchantSetupKey));
                
                if (merchantInfo == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found/Invalid Pin" };

                var eventLog = new EventRequestDto
                {
                    ModuleAccessed = EventLogProcess.MerchantSignUp,
                    Description = "Transaction Pin was successfully validated",
                    UserId = email,
                    ClientAuthenticationId = clientId
                };

                await _eventLogService.ActivityRequestLog(eventLog);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Validation was successful" };

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured to validating transaction PIN" + " | " + email + " | " + ex + " - " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured while setting transaction PIN" };
            }
        }

    }
}
