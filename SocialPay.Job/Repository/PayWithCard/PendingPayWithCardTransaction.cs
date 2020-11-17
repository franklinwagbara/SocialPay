using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
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
        public PendingPayWithCardTransaction(IServiceProvider services, 
            IOptions<AppSettings> appSettings, CreditDebitService creditDebitService)
        {
            Services = services;
           // _fioranoTransferRepository = fioranoTransferRepository;
            _appSettings = appSettings.Value;
            _creditDebitService = creditDebitService;
        }
        public IServiceProvider Services { get; }

        public async Task<WebApiResponse> InitiateTransactions(List<TransactionLog> pendingRequest)
        {
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    foreach (var item in pendingRequest)
                    {
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

                        var getWalletInfo = await context.MerchantWallet
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        if (getWalletInfo == null)
                            return null;

                        ////var initiateRequest = await _fioranoTransferRepository
                        ////    .InititiateDebit(Convert.ToString(getTransInfo.TotalAmount), 
                        ////    "Card-Payment" + " - " + item.TransactionReference +
                        ////    " - "+ item.CustomerTransactionReference, item.TransactionReference,
                        ////    "", false, item.PaymentChannel, "Card payment", requestId);


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
                            narrations = "CP from Nominated to Social Pay" + " - " + item.PaymentReference,
                            DebitAmount = Convert.ToString(getTransInfo.TotalAmount),
                            CreditAccountNo = _appSettings.socialT24AccountNo,
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
                            TransactionReference = item.TransactionReference,
                            DebitAmount = Convert.ToDouble(fioranoRequestBody.DebitAmount),
                            //DebitAmount = shippingInfo.TotalAmount, narrations = _appSettings.transactionNarration,
                            //DebitAmount = totalAmount,
                            narrations = fioranoRequestBody.narrations,
                            TransactionType = _appSettings.fioranoTransactionType,
                            TrxnLocation = _appSettings.fioranoTrxnLocation,
                            VtellerAppID = _appSettings.fioranoVtellerAppID,
                            Channel = item.PaymentChannel,
                            Message = "Card payment",
                            PaymentReference = item.PaymentReference
                        };
                        await context.FioranoT24Request.AddAsync(logRequest);
                        await context.SaveChangesAsync();
                        var postTransaction = await _creditDebitService.InitiateTransaction(jsonRequest);
                        
                        if (postTransaction.ResponseCode == AppResponseCodes.Success)
                        {
                           using(var transaction = await context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    getTransInfo.IsApproved = true;
                                    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.FioranoFirstFundingCompleted;
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
                                    return null;
                                }
                                catch (Exception ex)
                                {
                                    await transaction.RollbackAsync();
                                    throw;
                                }
                            }
                           
                        }
                        //getTransInfo.IsQueuedPayWithCard = false;
                        //getTransInfo.LastDateModified = DateTime.Now;
                        //context.Update(getTransInfo);
                        //await context.SaveChangesAsync();
                        return null;



                        ////if (initiateRequest.ResponseCode == AppResponseCodes.Success)
                        ////{
                        ////    getTransInfo.IsApproved = true;
                        ////    getTransInfo.IsCompletedPayWithCard = true;
                        ////    getTransInfo.LastDateModified = DateTime.Now;
                        ////    context.Update(getTransInfo);
                        ////    await context.SaveChangesAsync();
                        ////    return null;
                        ////}

                        ////getTransInfo.IsQueuedPayWithCard = false;
                        ////getTransInfo.LastDateModified = DateTime.Now;
                        ////context.Update(getTransInfo);
                        ////await context.SaveChangesAsync();
                        ////return null;
                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
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
