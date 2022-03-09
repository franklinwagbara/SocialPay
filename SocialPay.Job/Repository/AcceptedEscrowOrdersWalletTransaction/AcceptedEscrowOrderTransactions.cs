using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialPay.Helper.Dto.Request;
using SocialPay.Core.Configurations;
using Microsoft.Extensions.Options;
using SocialPay.Helper;
using SocialPay.Core.Services.Wallet;
using Microsoft.Data.SqlClient;
using SocialPay.Helper.SerilogService.Escrow;

namespace SocialPay.Job.Repository.AcceptedEscrowOrdersWalletTransaction
{
    public class AcceptedEscrowOrderTransactions
    {
        private readonly AppSettings _appSettings;
        private readonly WalletRepoJobService _walletRepoJobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AcceptedEscrowOrderTransactions));
        private readonly EscrowJobLogger _escrowLogger;
        public AcceptedEscrowOrderTransactions(IServiceProvider service, IOptions<AppSettings> appSettings,
            WalletRepoJobService walletRepoJobService, EscrowJobLogger escrowLogger)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _walletRepoJobService = walletRepoJobService;
            _escrowLogger = escrowLogger;
        }
        public IServiceProvider Services { get; }


        public async Task<WebApiResponse> ProcessTransactions(List<TransactionLog> pendingRequest)
        {
            long transactionLogid = 0;
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    foreach (var item in pendingRequest)
                    {
                        _escrowLogger.LogRequest($"{"Job Service. AcceptedEscrowOrderTransactions" + "-" + "ProcessTransactions request" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "  }{DateTime.Now}", false);

                        var requestId = Guid.NewGuid().ToString();
                        var getTransInfo = await context.TransactionLog
                            .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);

                        getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.ProcessingApprovedRequest;
                        getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.ProcessingApprovedRequest;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();

                        transactionLogid = getTransInfo.TransactionLogId;

                        var getWalletInfo = await context.MerchantWallet
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        
                        if (getWalletInfo == null)
                            return null;

                        var walletModel = new WalletTransferRequestDto
                        {
                            CURRENCYCODE = _appSettings.walletcurrencyCode,
                            amt = Convert.ToString(item.TotalAmount),
                            toacct = _appSettings.SterlingWalletPoolAccount,
                            channelID = 1,
                            TransferType = 1,
                            frmacct = getWalletInfo.Mobile,
                            paymentRef = item.PaymentReference,
                            remarks = "Social-Pay wallet transfer" + " - " + item.TransactionReference + " - " + item.Category
                        };

                        var walletRequestModel = new AcceptedEscrowWalletTransferRequestLog
                        {
                            amt = Convert.ToDecimal(walletModel.amt),
                            channelID = walletModel.channelID,
                            CURRENCYCODE = walletModel.CURRENCYCODE,
                            frmacct = walletModel.frmacct,
                            PaymentReference = walletModel.paymentRef,
                            remarks = walletModel.remarks,
                            toacct = walletModel.toacct,
                            TransactionReference = item.TransactionReference,
                            CustomerTransactionReference = item.CustomerTransactionReference,
                            TransferType = walletModel.TransferType,
                            RequestId = requestId,
                            ClientAuthenticationId = item.ClientAuthenticationId
                        };

                        await context.AcceptedEscrowWalletTransferRequestLog.AddAsync(walletRequestModel);
                        await context.SaveChangesAsync();

                        var initiateRequest = await _walletRepoJobService.WalletToWalletTransferAsync(walletModel);
                        if (initiateRequest.response == AppResponseCodes.Success)
                        {
                            using(var transaction = await context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    var walletResponse = new WalletTransferResponse
                                    {
                                        RequestId = walletRequestModel.RequestId,
                                        sent = initiateRequest.data.sent,
                                        message = initiateRequest.message,
                                        response = initiateRequest.response,
                                        PaymentReference = item.PaymentReference,
                                        responsedata = Convert.ToString(initiateRequest.responsedata),
                                    };

                                    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.WalletTranferCompleted;
                                    getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.WalletTranferCompleted;
                                    getTransInfo.LastDateModified = DateTime.Now;
                                    getTransInfo.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.WalletTranferCompleted;
                                    context.Update(getTransInfo);
                                    await context.SaveChangesAsync();
                                    await context.WalletTransferResponse.AddAsync(walletResponse);
                                    await context.SaveChangesAsync();
                                    await transaction.CommitAsync();
                                    _escrowLogger.LogRequest($"{"Job Service. AcceptedEscrowOrderTransactions" + "-" + "ProcessTransactions request was successful" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                                    return null;
                                }
                                catch (Exception ex)
                                {
                                    _escrowLogger.LogRequest($"{"Job Service. AcceptedEscrowOrderTransactions: An error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", false);
                                    await transaction.RollbackAsync();
                                    return null;
                                }
                            }
                        }

                        var failedResponse = new FailedTransactions
                        {
                            CustomerTransactionReference = item.CustomerTransactionReference,
                            Message = initiateRequest.message,
                            TransactionReference = item.TransactionReference
                        };
                        await context.FailedTransactions.AddAsync(failedResponse);
                        await context.SaveChangesAsync();
                        _escrowLogger.LogRequest($"{"Job Service. AcceptedEscrowOrderTransactions" + "-" + "ProcessTransactions request failed" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + failedResponse.Message + " | "}{DateTime.Now}", false);
                    }
                    return null;
                }

            }
            catch (Exception ex)
            {

                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    //using (var scope = Services.CreateScope())
                    //{
                    //    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    //    var getTransInfo = await context.TransactionLog
                    //      .SingleOrDefaultAsync(x => x.TransactionLogId == transactionLogid);

                    //    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.FioranoFirstFundingCompleted;
                    //    getTransInfo.LastDateModified = DateTime.Now;
                    //    context.Update(getTransInfo);
                    //    await context.SaveChangesAsync();
                    //}
                    _escrowLogger.LogRequest($"{"Job Service. AcceptedEscrowOrderTransactions: An error occured. Duplicate transaction reference" + " | " + transactionLogid + " | " + errorMessage + " | " + ex.Message.ToString() + " | " }{DateTime.Now}", false);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
