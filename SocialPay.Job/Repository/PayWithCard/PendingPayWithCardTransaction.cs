using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.Validations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.PayWithCardJob;
using SocialPay.Job.Repository.Fiorano;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.PayWithCard
{
    public class PendingPayWithCardTransaction
    {
        //private readonly FioranoTransferPayWithCardRepository _fioranoTransferRepository;
        private readonly AppSettings _appSettings;
        private readonly CreditDebitService _creditDebitService;
        private readonly BankServiceRepositoryJobService _bankServiceRepositoryJobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(PendingPayWithCardTransaction));
        private readonly PayWithCardJobLogger _paywithcardjobLogger;

        public PendingPayWithCardTransaction(IServiceProvider services, 
            IOptions<AppSettings> appSettings, CreditDebitService creditDebitService,
            BankServiceRepositoryJobService bankServiceRepositoryJobService, PayWithCardJobLogger paywithcardjobLogger)
        {
            Services = services;
            _appSettings = appSettings.Value;
            _creditDebitService = creditDebitService;
            _bankServiceRepositoryJobService = bankServiceRepositoryJobService;
            _paywithcardjobLogger = paywithcardjobLogger;
        }
        public IServiceProvider Services { get; }

        public async Task<WebApiResponse> InitiateTransactions(List<TransactionLog> pendingRequest)
        {
            long transactionLogid = 0;

            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();

                    foreach (var item in pendingRequest)
                    {
                        _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "Tasks starts to initiate name enquiry request" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                        var validateNuban = await _bankServiceRepositoryJobService.GetAccountFullInfoAsync(_appSettings.socialPayNominatedAccountNo, item.TotalAmount);

                        if (validateNuban.ResponseCode == AppResponseCodes.Success)
                        {
                            _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "InitiateTransactions request" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                            var requestId = Guid.NewGuid().ToString();
                            var getTransInfo = await context.TransactionLog
                                .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId
                                && x.TransactionJourney == TransactionJourneyStatusCodes.FirstWalletFundingWasSuccessul);

                            if (getTransInfo == null)
                                return null;

                            getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.FioranoFirstFundingProcessing;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();

                            transactionLogid = getTransInfo.TransactionLogId;

                            var getWalletInfo = await context.MerchantWallet
                                .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                            if (getWalletInfo == null)
                                return null;


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
                                narrations = $"{item.OtherPaymentReference}{"-"}{"CP from Nominated to Social Pay"}{ " - "}{item.PaymentReference}",
                                DebitAmount = Convert.ToString(getTransInfo.TotalAmount),
                                CreditAccountNo = _appSettings.socialT24AccountNo,
                            };






                            var request = new TransactionRequestDto { FT_Request = fioranoRequestBody };

                            var jsonRequest = JsonConvert.SerializeObject(request);

                            var logRequest = new FioranoT24CardCreditRequest
                            {
                                SessionId = fioranoRequestBody.SessionId,
                                CommissionCode = fioranoRequestBody.CommissionCode,
                                CreditAccountNo = fioranoRequestBody.CreditAccountNo,
                                CreditCurrency = _appSettings.fioranoCreditCurrency,
                                DebitCurrency = _appSettings.fioranoDebitCurrency,
                                JsonRequest = jsonRequest,
                                TransactionBranch = "NG0020006",
                                DebitAcctNo = fioranoRequestBody.DebitAcctNo,
                                TransactionReference = item.TransactionReference,
                                DebitAmount = Convert.ToDecimal(fioranoRequestBody.DebitAmount),
                                narrations = fioranoRequestBody.narrations,
                                TransactionType = _appSettings.fioranoTransactionType,
                                TrxnLocation = _appSettings.fioranoTrxnLocation,
                                VtellerAppID = _appSettings.fioranoVtellerAppID,
                                Channel = item.PaymentChannel,
                                Message = "Card payment transaction",
                                PaymentReference = item.PaymentReference
                            };

                            await context.FioranoT24CardCreditRequest.AddAsync(logRequest);
                            await context.SaveChangesAsync();

                            _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "FioranoT24CardCreditRequest request was successfully logged" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                            var postTransaction = await _creditDebitService.InitiateTransaction(jsonRequest);

                            if (postTransaction.ResponseCode == AppResponseCodes.Success)
                            {
                                using (var transaction = await context.Database.BeginTransactionAsync())
                                {
                                    try
                                    {
                                        getTransInfo.IsApproved = true;
                                        getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.FioranoFirstFundingCompleted;
                                        getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.FioranoFirstFundingCompleted;
                                        getTransInfo.LastDateModified = DateTime.Now;
                                        context.Update(getTransInfo);
                                        await context.SaveChangesAsync();

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
                                        await transaction.CommitAsync();

                                        _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "PendingPayWithCardTransaction request was successfully updated" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                                        return null;
                                    }
                                    catch (Exception ex)
                                    {
                                        await transaction.RollbackAsync();
                                        throw;
                                    }
                                }

                            }
                            else
                            {
                                //Log failed response here
                                






                            }
                           
                            return null;

                        }

                        else
                        {
                            var failedResponse = new FailedTransactions
                            {
                                CustomerTransactionReference = item.CustomerTransactionReference,
                                Message = "Name enquiry issues" + "-"+ validateNuban.UsableBal + "-"+ item.TotalAmount + "-"+ validateNuban.ResponseCode +"-"+ item.PaymentReference,
                                TransactionReference = item.TransactionReference
                            };

                            await context.FailedTransactions.AddAsync(failedResponse);
                            await context.SaveChangesAsync();
                        }

                    }

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (SqlException db)
            {
                _paywithcardjobLogger.LogRequest($"{"An error occured. Duplicate transaction reference" + " | " + transactionLogid + " | " + db.Message + " | "}{DateTime.Now}", true);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

            catch (Exception ex)
            {
                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;

                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    _paywithcardjobLogger.LogRequest($"{"An error occured. Duplicate transaction reference" + " | " + transactionLogid + " | " + errorMessage + " | " + ex.Message.ToString() + " | " }{DateTime.Now}", true);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
