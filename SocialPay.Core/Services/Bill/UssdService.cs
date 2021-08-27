using Microsoft.Extensions.Options;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Bill
{
    public class UssdService
    {
        private readonly IUssdRequestLogService _ussdServiceLogRequestService;
        private readonly AppSettings _appSettings;
        private readonly UssdApiService _ussdApiService;
        public UssdService(IUssdRequestLogService ussdServiceLogRequestService,
            IOptions<AppSettings> appSettings, UssdApiService UssdApiService)
        {
            _ussdApiService = UssdApiService ?? throw new ArgumentNullException(nameof(UssdApiService));
            _ussdServiceLogRequestService = ussdServiceLogRequestService ?? throw new ArgumentNullException(nameof(ussdServiceLogRequestService));
            _appSettings = appSettings.Value;
        }
        public async Task<WebApiResponse> GeneratePaymentReference(GenerateReferenceDTO generateReferenceDTO, long clientId, string reference)
        {
            try
            {
                var generator = new Random();

                var model = new GenerateReferenceRequestDTO
                {
                    merchantID = _appSettings.UssdMerchantID,
                    amount = generateReferenceDTO.amount,
                    callbackUrl = _appSettings.UssdCallbackUrl,
                    channel = _appSettings.UssdChannel,
                    merchantName = _appSettings.UssdMerchantName,
                    transactionType = _appSettings.UssdTransactionType,
                    transRef = generator.Next(100000, 1000000).ToString() + "" + generator.Next(100000, 1000000).ToString()
                };

                var ussdModel = new UssdRequestViewModel
                {
                    MerchantID = model.merchantID,
                    Channel = model.channel,
                    ClientAuthenticationId = clientId,
                    TransactionType = model.transactionType,
                    TransRef = model.transRef,
                    Amount = Convert.ToDouble(model.amount),
                    MerchantName = model.merchantName,
                    PaymentReference = reference
                };

                var logRequest = await _ussdServiceLogRequestService.AddAsync(ussdModel);

                var generateReference = await _ussdApiService.InitiateNewPaymentReference(model, clientId);

                if (generateReference.ResponseCode != AppResponseCodes.Success)
                    return new WebApiResponse { ResponseCode = generateReference.ResponseCode, Data = generateReference };

                ussdModel.UssdServiceRequestLogId = logRequest.UssdServiceRequestLogId;
                ussdModel.TraceID = generateReference.ResponseDetails.TraceID;
                ussdModel.TransactionID = generateReference.ResponseDetails.TransactionID;
                ussdModel.TransactionRef = generateReference.ResponseDetails.Reference;
                ussdModel.ResponseCode = generateReference.ResponseHeader.ResponseCode;
                ussdModel.ResponseMessage = generateReference.ResponseHeader.ResponseMessage;

                await _ussdServiceLogRequestService.UpdateAsync(ussdModel);

                return new WebApiResponse { ResponseCode = generateReference.ResponseCode, Data = generateReference };

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> PaymentRequery(string paymentReference)
        {
            try
            {
                var generator = new Random();

                var ussdRequestModel = await _ussdServiceLogRequestService.GetTransactionByreference(paymentReference);


                var request = new GatewayRequeryRequestDTO
                {
                    TransactionID = ussdRequestModel.TransactionID,
                    merchantID = _appSettings.UssdGatewayRequeryMerchantID,
                    terminalID = _appSettings.UssdTerminalID,
                    amount = Convert.ToString(ussdRequestModel.Amount)
                };

                var confirmTransaction = await _ussdApiService.UssdTransactionRequery(request);

                if (confirmTransaction.responseCode != AppResponseCodes.Success)
                    return new WebApiResponse { ResponseCode = confirmTransaction.responseCode, Data = confirmTransaction };

                ussdRequestModel.UssdServiceRequestLogId = ussdRequestModel.UssdServiceRequestLogId;
                ussdRequestModel.TraceID = confirmTransaction.TraceID;
                ussdRequestModel.TransactionID = confirmTransaction.TransactionID;
                ussdRequestModel.TransactionRef = confirmTransaction.reference;
                ussdRequestModel.ResponseCode = confirmTransaction.responseCode;
                ussdRequestModel.ResponseMessage = confirmTransaction.responsemessage;

                await _ussdServiceLogRequestService.UpdateAsync(ussdRequestModel);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Request was successful" };

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }

}
