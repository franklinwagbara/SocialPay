﻿using Microsoft.Data.SqlClient;
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
    public class FioranoTransferPayWithCardRepository
    {
        private readonly CreditDebitService _creditDebitService;
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(FioranoTransferPayWithCardRepository));
        private readonly FioranoJobLogger _fioranoLogger;

        public FioranoTransferPayWithCardRepository(IOptions<AppSettings> appSettings, CreditDebitService creditDebitService,
            IServiceProvider services, FioranoJobLogger fioranoLogger)
        {
            _appSettings = appSettings.Value;
            _creditDebitService = creditDebitService;
            Services = services;
            _fioranoLogger = fioranoLogger;
        }

        public IServiceProvider Services { get; }
        public async Task<WebApiResponse> InititiateDebit(string debitAmount, string narration,
            string transactionRef, string creditAccountNo, bool tranType, string channel,
            string message, string paymentReference, long transactionLogid)
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
                        narrations = narration,
                        DebitAmount = debitAmount,
                        CreditAccountNo = _appSettings.socialT24AccountNo,
                    };
                    if(tranType)
                    {
                        fioranoRequestBody.CreditAccountNo = creditAccountNo;
                        fioranoRequestBody.DebitAcctNo = _appSettings.socialT24AccountNo;
                    }
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
                        TransactionReference = transactionRef,
                        DebitAmount = Convert.ToDecimal(fioranoRequestBody.DebitAmount),
                        narrations = narration,
                        TransactionType = _appSettings.fioranoTransactionType,
                        TrxnLocation = _appSettings.fioranoTrxnLocation,
                        VtellerAppID = _appSettings.fioranoVtellerAppID,
                        Channel = channel, Message = message,
                        PaymentReference = paymentReference
                    };
                    await context.FioranoT24CardCreditRequest.AddAsync(logRequest);
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
                _fioranoLogger.LogRequest($"{"Job Service" + "-" + "Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", false);

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
                        _fioranoLogger.LogRequest($"{"Job Service" + "-" + "Error occured. Duplicate transaction" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | " }{DateTime.Now}", false);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                    }
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> InititiateMerchantCredit(string debitAmount, string narration,
           string transactionRef, string creditAccountNo, bool tranType, string channel,
           string message, string paymentReference)
        {
            _fioranoLogger.LogRequest($"{"Job Service" + "-" + "InititiateMerchantCredit fiorano request" + " | " + transactionRef + " | " + paymentReference + " | " + creditAccountNo + " | " + debitAmount + " | "}{DateTime.Now}", false);

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
                        CreditAccountNo = _appSettings.socialT24AccountNo,
                    };

                    if (tranType)
                    {
                        fioranoRequestBody.CreditAccountNo = creditAccountNo;
                        fioranoRequestBody.DebitAcctNo = _appSettings.socialT24AccountNo;
                    }

                    var request = new TransactionRequestDto { FT_Request = fioranoRequestBody };
                    var jsonRequest = JsonConvert.SerializeObject(request);

                    var logRequest = new NonEscrowFioranoT24Request
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

                    await context.NonEscrowFioranoT24Request.AddAsync(logRequest);
                    await context.SaveChangesAsync();

                    var postTransaction = await _creditDebitService.InitiateTransaction(jsonRequest);

                    _fioranoLogger.LogRequest($"{"Job Service" + "-" + "InititiateMerchantCredit fiorano base response" + " | " + transactionRef + " | " + paymentReference + " | " + postTransaction.FTResponse + " | " + postTransaction.Message + " | "}{DateTime.Now}", false);

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
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = db.Message.ToString() };
            }

            catch (Exception ex)
            {
                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    _fioranoLogger.LogRequest($"{"An error occured. Duplicate transaction reference" + " | " + transactionRef + " | " + paymentReference + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction, Data = errorMessage };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = errorMessage };
            }
        }



     
    }
}
