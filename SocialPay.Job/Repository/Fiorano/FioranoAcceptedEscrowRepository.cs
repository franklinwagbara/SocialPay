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
using SocialPay.Helper.SerilogService.FioranoJob;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.Fiorano
{
    public class FioranoAcceptedEscrowRepository
    {
        private readonly CreditDebitService _creditDebitService;
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(FioranoAcceptedEscrowRepository));
        private readonly FioranoJobLogger _fioranoLogger;
        public FioranoAcceptedEscrowRepository(IOptions<AppSettings> appSettings, CreditDebitService creditDebitService,
            IServiceProvider services, FioranoJobLogger fioranoLogger)
        {
            _appSettings = appSettings.Value;
            _creditDebitService = creditDebitService;
            Services = services;
            _fioranoLogger = fioranoLogger;
        }

        public IServiceProvider Services { get; }


        public async Task<WebApiResponse> InititiateEscrowAcceptedRequest(string debitAmount, string narration,
         string transactionRef, string creditAccountNo, string channel,
         string message, string paymentReference)
        {
            _fioranoLogger.LogRequest($"{"Job Service: InititiateEscrowAcceptedRequest task starts" + " | " + paymentReference + " | "}{DateTime.Now}", false);
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
                        DebitAcctNo = _appSettings.socialT24AccountNo,
                        TransactionBranch = "NG0020006",
                        narrations = narration,
                        DebitAmount = debitAmount,
                        CreditAccountNo = creditAccountNo,
                    };

                  
                    var request = new TransactionRequestDto { FT_Request = fioranoRequestBody };
                    var jsonRequest = JsonConvert.SerializeObject(request);

                    var logRequest = new AcceptedEscrowFioranoT24Request
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
                        Channel = channel,
                        Message = message,
                        PaymentReference = paymentReference
                    };
                    await context.AcceptedEscrowFioranoT24Request.AddAsync(logRequest);
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
                _fioranoLogger.LogRequest($"{"Job Service: An error occured. Base error" + " | " + paymentReference + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);

                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    _fioranoLogger.LogRequest($"{"Job Service: An error occured. Duplicate transaction reference" + " | " + paymentReference + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
