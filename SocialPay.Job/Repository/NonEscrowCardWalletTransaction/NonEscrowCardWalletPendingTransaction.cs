using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.Wallet;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.NonEscrowJob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowCardWalletTransaction
{
    public class NonEscrowCardWalletPendingTransaction
    {
        private readonly AppSettings _appSettings;
        private readonly WalletRepoJobService _walletRepoJobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NonEscrowCardWalletPendingTransaction));
        private readonly NonEscrowJobLogger _nonescrowLogger;
        public NonEscrowCardWalletPendingTransaction(IServiceProvider service, IOptions<AppSettings> appSettings,
            WalletRepoJobService walletRepoJobService, NonEscrowJobLogger nonescrowLogger)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _walletRepoJobService = walletRepoJobService;
            _nonescrowLogger = nonescrowLogger;
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
                        _nonescrowLogger.LogRequest($"{"Job Service" + "-" + "Non Escrow Card Wallet Pending Transaction request" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                        var requestId = Guid.NewGuid().ToString();
                        var getTransInfo = await context.TransactionLog
                            .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);

                        if (getTransInfo == null)
                            return null;

                        getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.ProcessingFinalWalletRequest;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);

                        await context.SaveChangesAsync();

                        transactionLogid = getTransInfo.TransactionLogId;

                        var getWalletInfo = await context.MerchantWallet
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                        if (getWalletInfo == null)
                        {
                            _nonescrowLogger.LogRequest($"{"Job Service" + "-" + "Non Escrow Card Wallet Pending Transaction wallet info is null" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                            return null;
                        }

                        var walletModel = new WalletTransferRequestDto
                        {
                            CURRENCYCODE = _appSettings.walletcurrencyCode,
                            amt = Convert.ToString(item.TotalAmount),
                            toacct = _appSettings.SterlingWalletPoolAccount,
                            channelID = 1,
                            TransferType = 1,
                            frmacct = getWalletInfo.Mobile,
                            paymentRef = item.PaymentReference,
                            remarks = $"{"Social-Pay wallet transfer"}{" - " }{item.PaymentReference}{" - "}{item.TransactionReference}{" - " + item.Category}"
                        };

                        var walletRequestModel = new DebitMerchantWalletTransferRequestLog
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

                        await context.DebitMerchantWalletTransferRequestLog.AddAsync(walletRequestModel);

                        await context.SaveChangesAsync();

                        _nonescrowLogger.LogRequest($"{"Job Service" + "-" + "Saved default wallet details. Trying  to log wallet transfer" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

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
                                        PaymentReference = item.PaymentReference
                                    };

                                    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.WalletTranferCompleted;
                                    getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.WalletTranferCompleted;
                                    getTransInfo.LastDateModified = DateTime.Now;
                                    context.Update(getTransInfo);
                                    await context.SaveChangesAsync();

                                    await context.WalletTransferResponse.AddAsync(walletResponse);
                                    await context.SaveChangesAsync();

                                    await transaction.CommitAsync();

                                    _nonescrowLogger.LogRequest($"{"Job Service" + "-" + "Non Escrow Card Wallet Pending Transaction successfully updated" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                                    return null;
                                }
                                catch (Exception ex)
                                {
                                    _nonescrowLogger.LogRequest($"{"Job Service" + "-" + "Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | " }{DateTime.Now}", true);

                                    await transaction.RollbackAsync();
                                    return null;
                                }
                            }
                        }

                        _nonescrowLogger.LogRequest($"{"Job Service" + "-" + "Non Escrow Card Wallet Pending Transaction Failed" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " }{DateTime.Now}", false);


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
                _nonescrowLogger.LogRequest($"{"Job Service" + "-" + "Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);

                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    ////using (var scope = Services.CreateScope())
                    ////{
                    ////    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    ////    var getTransInfo = await context.TransactionLog
                    ////      .SingleOrDefaultAsync(x => x.TransactionLogId == transactionLogid);

                    ////    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.WalletTranferCompleted;
                    ////    getTransInfo.LastDateModified = DateTime.Now;
                    ////    context.Update(getTransInfo);
                    ////    await context.SaveChangesAsync();
                    ////}
                    _nonescrowLogger.LogRequest($"{"An error occured. Duplicate transaction reference" + " | " + transactionLogid + " | " + errorMessage + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
