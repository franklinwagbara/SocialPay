using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

namespace SocialPay.Core.Services.Fiorano
{
    public class FioranoService
    {
        private readonly FioranoAPIService _fioranoService;
        private readonly IFioranoRequestService _fioranoRequestService;
        private readonly IFioranoResponseService _fioranoResponseService;
        private readonly IMerchantBankingInfoService _merchantBankingInfoService;
        private readonly AppSettings _appSettings;
        public FioranoService(FioranoAPIService fioranoService, IFioranoRequestService fioranoRequestService,
            IOptions<AppSettings> appSettings, IFioranoResponseService fioranoResponseService,
            IMerchantBankingInfoService merchantBankingInfoService)
        {
            _appSettings = appSettings.Value;
            _fioranoService = fioranoService ?? throw new ArgumentNullException(nameof(fioranoService));
            _fioranoRequestService = fioranoRequestService ?? throw new ArgumentNullException(nameof(fioranoRequestService));
            _fioranoResponseService = fioranoResponseService ?? throw new ArgumentNullException(nameof(fioranoResponseService));
            _merchantBankingInfoService = merchantBankingInfoService ?? throw new ArgumentNullException(nameof(merchantBankingInfoService));
        }

        public async Task<WebApiResponse> InitiateFioranoRequest(FioranoBillsRequestDto fioranoBillsRequestDto, long clientId)
        {
            try
            {
                var bankInfo = await _merchantBankingInfoService.GetMerchantBankInfo(clientId);

                if (bankInfo == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Banking info not found" };

                ////if(bankInfo.BankCode != _appSettings.SterlingBankCode)
                ////    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Merchant bank must be Sterling bank to complete this request" };

                bankInfo.Nuban = "0065428109";

                var model = new FTRequest
                {
                    SessionId = Guid.NewGuid().ToString(),
                    CommissionCode = _appSettings.fioranoCommisionCode,
                    CreditCurrency = _appSettings.fioranoCreditCurrency,
                    DebitCurrency = _appSettings.fioranoCreditCurrency,
                    VtellerAppID = _appSettings.fioranoVtellerAppID,
                    TrxnLocation = _appSettings.fioranoTrxnLocation,
                    TransactionType = _appSettings.fioranoTransactionType,
                    DebitAcctNo = bankInfo.Nuban,
                    TransactionBranch = "NG0020006",
                    narrations = "Social Pay Bills Payment",
                    DebitAmount = fioranoBillsRequestDto.DebitAmount,
                    CreditAccountNo = _appSettings.socialT24AccountNo,
                };

                var requestModel = new FioranoRequestViewModel
                {
                    SessionId = model.SessionId,
                    CommissionCode = model.CommissionCode,
                    CreditCurrency = model.CreditCurrency,
                    DebitCurrency = model.DebitCurrency,
                    VtellerAppID = model.VtellerAppID,
                    TrxnLocation = model.TrxnLocation,
                    TransactionType = model.TransactionType,
                    DebitAcctNo = model.DebitAcctNo,
                    TransactionBranch = "NG0020006",
                    narrations = "Social Pay Bills Payment",
                    DebitAmount = model.DebitAmount,
                    CreditAccountNo = model.CreditAccountNo,
                    ClientAuthenticationId = clientId
                };

                var logRequest = await _fioranoRequestService.AddAsync(requestModel);

                var request = new TransactionRequestDto { FT_Request = model };

                var jsonRequest = JsonConvert.SerializeObject(request);

                var debitCustomer = await _fioranoService.InitiateTransaction(jsonRequest);

                var response = new FioronoBillsPaymentResponseViewModel();

                if (debitCustomer.ResponseCode == AppResponseCodes.Success)
                {
                    response.PaymentReference = fioranoBillsRequestDto.TransactionReference;
                    response.ReferenceID = debitCustomer.FTResponse.ReferenceID;
                    response.Balance = debitCustomer.FTResponse.Balance;
                    response.CHARGEAMT = debitCustomer.FTResponse.CHARGEAMT;
                    response.COMMAMT = debitCustomer.FTResponse.COMMAMT;
                    response.FTID = debitCustomer.FTResponse.FTID;
                    response.ResponseCode = debitCustomer.FTResponse.ResponseCode;
                    response.ResponseText = debitCustomer.FTResponse.ResponseText;
                    response.FioranoBillsRequestId = logRequest.FioranoBillsRequestId;

                    await _fioranoResponseService.AddAsync(response);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = debitCustomer, Message = "Success" };

                }


                response.PaymentReference = fioranoBillsRequestDto.TransactionReference;
                response.ReferenceID = debitCustomer.FTResponse.ReferenceID;
                response.ResponseCode = debitCustomer.FTResponse.ResponseCode;
                response.ResponseText = debitCustomer.FTResponse.ResponseText;
                response.JsonResponse = debitCustomer.Message;
                response.FioranoBillsRequestId = logRequest.FioranoBillsRequestId;

                await _fioranoResponseService.AddAsync(response);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
