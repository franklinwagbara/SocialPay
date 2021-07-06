using Microsoft.Data.SqlClient;
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
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.Fiorano
{
    public class DeliveryDayFioranoTransferRepository
    {
        private readonly CreditDebitService _creditDebitService;
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(DeliveryDayFioranoTransferRepository));

        public DeliveryDayFioranoTransferRepository(IOptions<AppSettings> appSettings, CreditDebitService creditDebitService,
            IServiceProvider services)
        {
            _appSettings = appSettings.Value;
            _creditDebitService = creditDebitService;
            Services = services;
        }

        public IServiceProvider Services { get; }

        public async Task<WebApiResponse> InititiateDebit(string debitAmount, string narration,
            string transactionRef, string creditAccountNo, string channel,
            string message, string paymentReference, long transactionLogid)
        {
            _log4net.Info("Job Service" + "-" + "InititiateDebit task" + " | " + paymentReference + " | " + creditAccountNo + " | " + DateTime.Now);

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
                        narrations = narration,
                        DebitAmount = debitAmount,
                        CreditAccountNo = creditAccountNo,
                    };
                  
                    var request = new TransactionRequestDto { FT_Request = fioranoRequestBody };
                    var jsonRequest = JsonConvert.SerializeObject(request);

                    var logRequest = new FioranoT24DeliveryDayRequest
                    {
                        SessionId = fioranoRequestBody.SessionId,
                        CommissionCode = fioranoRequestBody.CommissionCode,
                        CreditAccountNo = fioranoRequestBody.CreditAccountNo,
                        CreditCurrency = _appSettings.fioranoCreditCurrency,
                        DebitCurrency = _appSettings.fioranoDebitCurrency,
                        JsonRequest = jsonRequest,
                        TransactionBranch = "NG0020006",
                        DebitAcctNo = fioranoRequestBody.DebitAcctNo,
                        TransactionReference = transactionRef,
                        DebitAmount = Convert.ToDecimal(fioranoRequestBody.DebitAmount),
                        narrations = narration,
                        TransactionType = _appSettings.fioranoTransactionType,
                        TrxnLocation = _appSettings.fioranoTrxnLocation,
                        VtellerAppID = _appSettings.fioranoVtellerAppID,
                        Channel = channel, Message = message,
                        PaymentReference = paymentReference
                    };

                    await context.FioranoT24DeliveryDayRequest.AddAsync(logRequest);
                    await context.SaveChangesAsync();

                    var postTransaction = await _creditDebitService.InitiateTransaction(jsonRequest);
                    var logFioranoResponse = new FioranoT24TransactionResponse
                    {
                        PaymentReference = logRequest.PaymentReference,
                        Balance = postTransaction.FTResponse.Balance,
                        CHARGEAMT = postTransaction.FTResponse.CHARGEAMT,
                        COMMAMT = postTransaction.FTResponse.COMMAMT,
                        FTID = postTransaction.FTResponse.FTID,
                        JsonResponse = postTransaction.Message,
                        ReferenceID = postTransaction.FTResponse.ReferenceID,
                        ResponseCode = postTransaction.FTResponse.ResponseCode,
                        ResponseText = postTransaction.FTResponse.ResponseText
                    };
                    await context.FioranoT24TransactionResponse.AddAsync(logFioranoResponse);
                    await context.SaveChangesAsync();

                    if (postTransaction.ResponseCode == AppResponseCodes.Success)
                    {                        
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                }
               
            }
            catch (SqlException db)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

            catch (Exception ex)
            {
                _log4net.Error("Job Service" + "-" + "Error occured" + " | " + paymentReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                using (var scope = Services.CreateScope())
                {
                    if (errorMessage.Contains("Violation") || code == 2627)
                    {
                        //using (var scope = Services.CreateScope())
                        //{
                        //    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                        //    var getTransInfo = await context.TransactionLog
                        //      .SingleOrDefaultAsync(x => x.TransactionLogId == transactionLogid);

                        //    getTransInfo.OrderStatus = TransactionJourneyStatusCodes.CompletedWalletFunding;
                        //    getTransInfo.LastDateModified = DateTime.Now;
                        //    context.Update(getTransInfo);
                        //    await context.SaveChangesAsync();
                        //}

                        _log4net.Error("Job Service" + "-" + "Error occured. Duplicate transaction" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                    }

                    // await context.SaveChangesAsync();
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
