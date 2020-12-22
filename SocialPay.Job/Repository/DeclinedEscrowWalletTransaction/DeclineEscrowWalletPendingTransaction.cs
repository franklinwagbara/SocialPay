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

namespace SocialPay.Job.Repository.DeclinedEscrowWalletTransaction
{
    public class DeclineEscrowWalletPendingTransaction
    {
        private readonly AppSettings _appSettings;
        private readonly WalletRepoJobService _walletRepoJobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(DeclineEscrowWalletPendingTransaction));

        public DeclineEscrowWalletPendingTransaction(IServiceProvider service, IOptions<AppSettings> appSettings,
            WalletRepoJobService walletRepoJobService)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _walletRepoJobService = walletRepoJobService;
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
                        _log4net.Info("Job Service" + "-" + "DeclineEscrowWalletPendingTransaction" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                        var requestId = Guid.NewGuid().ToString();
                        var getTransInfo = await context.TransactionLog
                            .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);

                        getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.ProcessingRejectedRequest;
                        getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.ProcessingRejectedRequest;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();

                        var getWalletInfo = await context.MerchantWallet
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        if (getWalletInfo == null)
                            return null;

                        transactionLogid = getTransInfo.TransactionLogId;
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

                        var walletRequestModel = new DeclinedWalletTransferRequestLog
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

                        await context.DeclinedWalletTransferRequestLog.AddAsync(walletRequestModel);
                        await context.SaveChangesAsync();

                        var initiateRequest = await _walletRepoJobService.WalletToWalletTransferAsync(walletModel);
                        if (initiateRequest.response == AppResponseCodes.Success)
                        {
                            using (var transaction = await context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    var walletResponse = new WalletTransferResponse
                                    {
                                        RequestId = walletRequestModel.RequestId,
                                        sent = initiateRequest.data.sent,
                                        message = initiateRequest.message,
                                        response = initiateRequest.response,
                                        responsedata = Convert.ToString(initiateRequest.responsedata),
                                    };

                                    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.WalletTranferCompletedForRefund;
                                    getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.WalletTranferCompletedForRefund;
                                    getTransInfo.LastDateModified = DateTime.Now;
                                    getTransInfo.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.WalletTranferCompletedForRefund;
                                    context.Update(getTransInfo);
                                    await context.SaveChangesAsync();
                                    await context.WalletTransferResponse.AddAsync(walletResponse);
                                    await context.SaveChangesAsync();
                                    await transaction.CommitAsync();
                                    _log4net.Info("Job Service" + "-" + "DeclineEscrowWalletPendingTransaction successful" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                                    return null;
                                }
                                catch (Exception ex)
                                {
                                    _log4net.Error("Job Service" + "-" + "Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | " + DateTime.Now);

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
                    }
                    return null;
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Job Service" + "-" + "Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | " + DateTime.Now);

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

                    //    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.WalletTranferCompleted;
                    //    getTransInfo.LastDateModified = DateTime.Now;
                    //    context.Update(getTransInfo);
                    //    await context.SaveChangesAsync();
                    //}

                    //_log4net.Error("An error occured. Duplicate transaction reference" + " | " + transferRequestDto.TransactionReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
