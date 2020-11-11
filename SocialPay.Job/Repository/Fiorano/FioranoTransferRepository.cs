using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.Fiorano
{
    public class FioranoTransferRepository
    {
        private readonly CreditDebitService _creditDebitService;
        private readonly AppSettings _appSettings;
        public FioranoTransferRepository(IOptions<AppSettings> appSettings, CreditDebitService creditDebitService,
            IServiceProvider services)
        {
            _appSettings = appSettings.Value;
            _creditDebitService = creditDebitService;
            Services = services;
        }

        public IServiceProvider Services { get; }
        public async Task<WebApiResponse> InititiateDebit(string debitAmount)
        {
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    var fioranoRequestBody = new FTRequest
                    {
                        SessionId = Guid.NewGuid().ToString(),
                        CommissionCode = _appSettings.fioranoCommisionCode,
                        CreditCurrency = _appSettings.fioranoCreditCurrency,
                        DebitCurrency = _appSettings.fioranoCreditCurrency,
                        VtellerAppID = _appSettings.fioranoVtellerAppID,
                        TrxnLocation = _appSettings.fioranoTrxnLocation,
                        TransactionType = _appSettings.fioranoTransactionType,
                        DebitAcctNo = _appSettings.socialPayNominatedAccountNo,
                        TransactionBranch = "NG0020006",
                        narrations = _appSettings.transactionNarration,
                        DebitAmount = debitAmount,
                        CreditAccountNo = _appSettings.socialT24AccountNo
                        // DebitAmount = shippingInfo.TotalAmount.ToString(), CreditAccountNo = _appSettings.altmallCollectionAccount
                       // DebitAmount = totalAmount.ToString(),
                        //CreditAccountNo = _appSettings.altmallCollectionAccount
                    };
                    var request = new TransactionRequestDto { FT_Request = fioranoRequestBody };
                    var jsonRequest = JsonConvert.SerializeObject(request);
                    var logRequest = new FioranoT24Request
                    {
                        SessionId = fioranoRequestBody.SessionId,
                        CommissionCode = fioranoRequestBody.CommissionCode,
                        CreditAccountNo = fioranoRequestBody.CreditAccountNo,
                        CreditCurrency = _appSettings.fioranoCreditCurrency,
                        DebitCurrency = _appSettings.fioranoDebitCurrency,
                        JsonRequest = jsonRequest,
                        TransactionBranch = "NG0020006",
                        DebitAcctNo = fioranoRequestBody.DebitAcctNo,
                        DebitAmount = Convert.ToDouble(fioranoRequestBody.DebitAmount),
                        //DebitAmount = shippingInfo.TotalAmount, narrations = _appSettings.transactionNarration,
                        //DebitAmount = totalAmount,
                        narrations = _appSettings.transactionNarration,
                        TransactionType = _appSettings.fioranoTransactionType,
                        TrxnLocation = _appSettings.fioranoTrxnLocation,
                        VtellerAppID = _appSettings.fioranoVtellerAppID,
                    };
                    await context.FioranoT24Request.AddAsync(logRequest);
                    await context.SaveChangesAsync();
                    var postTransaction = await _creditDebitService.InitiateTransaction(jsonRequest);
                    if(postTransaction.ResponseCode == AppResponseCodes.Success)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                }
               
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
