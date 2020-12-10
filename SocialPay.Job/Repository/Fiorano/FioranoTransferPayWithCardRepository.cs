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
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.Fiorano
{
    public class FioranoTransferPayWithCardRepository
    {
        private readonly CreditDebitService _creditDebitService;
        private readonly AppSettings _appSettings;
        public FioranoTransferPayWithCardRepository(IOptions<AppSettings> appSettings, CreditDebitService creditDebitService,
            IServiceProvider services)
        {
            _appSettings = appSettings.Value;
            _creditDebitService = creditDebitService;
            Services = services;
        }

        public IServiceProvider Services { get; }
        public async Task<WebApiResponse> InititiateDebit(string debitAmount, string narration,
            string transactionRef, string creditAccountNo, bool tranType, string channel,
            string message, string paymentReference)
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
                        DebitAmount = Convert.ToDouble(fioranoRequestBody.DebitAmount),
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
                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    //_log4net.Error("An error occured. Duplicate transaction reference" + " | " + transferRequestDto.TransactionReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> InititiateMerchantCredit(string debitAmount, string narration,
           string transactionRef, string creditAccountNo, bool tranType, string channel,
           string message, string paymentReference)
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
                        DebitAmount = Convert.ToDouble(fioranoRequestBody.DebitAmount),
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
                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    //_log4net.Error("An error occured. Duplicate transaction reference" + " | " + transferRequestDto.TransactionReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }



        public async Task<WebApiResponse> InititiateEscrowAcceptedRequest(string debitAmount, string narration,
          string transactionRef, string creditAccountNo, bool tranType, string channel,
          string message, string paymentReference)
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
                    if (tranType)
                    {
                        fioranoRequestBody.CreditAccountNo = creditAccountNo;
                        fioranoRequestBody.DebitAcctNo = _appSettings.socialT24AccountNo;
                    }
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
                        DebitAmount = Convert.ToDouble(fioranoRequestBody.DebitAmount),
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
                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    //_log4net.Error("An error occured. Duplicate transaction reference" + " | " + transferRequestDto.TransactionReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
