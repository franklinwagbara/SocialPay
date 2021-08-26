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
        public async Task<WebApiResponse> GeneratePaymentReference(GenerateReferenceDTO generateReferenceDTO, long clientId)
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
                    MerchantName = model.merchantName

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

        //public async Task<WebApiResponse> PaymentRequery(GatewayRequeryDTO model, long clientId)
        //{
        //    try
        //    {
        //        var generator = new Random();

        //        var ussdRequest = await _ussdServiceLogRequestService.GetUssdByClientId

        //        var model = new GenerateReferenceRequestDTO
        //        {
        //            merchantID = _appSettings.UssdMerchantID,
        //            amount = generateReferenceDTO.amount,
        //            callbackUrl = _appSettings.UssdCallbackUrl,
        //            channel = _appSettings.UssdChannel,
        //            merchantName = _appSettings.UssdMerchantName,
        //            transactionType = _appSettings.UssdTransactionType,
        //            transRef = generator.Next(100000, 1000000).ToString() + "" + generator.Next(100000, 1000000).ToString()
        //        };

        //        var ussdModel = new UssdRequestViewModel
        //        {
        //            MerchantID = model.merchantID,
        //            Channel = model.channel,
        //            ClientAuthenticationId = clientId,
        //            TransactionType = model.transactionType,
        //            TransRef = model.transRef,
        //            Amount = Convert.ToDouble(model.amount),
        //            MerchantName = model.merchantName

        //        };

        //        var logRequest = await _ussdServiceLogRequestService.AddAsync(ussdModel);

        //        var generateReference = await _ussdApiService.InitiateNewPaymentReference(model, clientId);

        //        if (generateReference.ResponseCode != AppResponseCodes.Success)
        //            return new WebApiResponse { ResponseCode = generateReference.ResponseCode, Data = generateReference };

        //        ussdModel.UssdServiceRequestLogId = logRequest.UssdServiceRequestLogId;
        //        ussdModel.TraceID = generateReference.ResponseDetails.TraceID;
        //        ussdModel.TransactionID = generateReference.ResponseDetails.TransactionID;
        //        ussdModel.TransactionRef = generateReference.ResponseDetails.Reference;
        //        ussdModel.ResponseCode = generateReference.ResponseHeader.ResponseCode;
        //        ussdModel.ResponseMessage = generateReference.ResponseHeader.ResponseMessage;

        //        await _ussdServiceLogRequestService.UpdateAsync(ussdModel);

        //        return new WebApiResponse { ResponseCode = generateReference.ResponseCode, Data = generateReference };

        //    }
        //    catch (Exception ex)
        //    {
        //        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
        //    }
        //}

    }

}
