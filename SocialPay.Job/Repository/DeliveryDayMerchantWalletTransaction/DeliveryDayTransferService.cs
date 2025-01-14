﻿using Microsoft.Data.SqlClient;
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
using SocialPay.Helper.SerilogService.WalletJob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeliveryDayMerchantWalletTransaction
{
    public class DeliveryDayTransferService
    {
        private readonly AppSettings _appSettings;
        private readonly WalletRepoJobService _walletRepoJobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(DeliveryDayTransferService));
        private readonly WalletJobLogger _walletLogger;

        public DeliveryDayTransferService(IServiceProvider service, IOptions<AppSettings> appSettings,
         WalletRepoJobService walletRepoJobService, WalletJobLogger walletLogger)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _walletRepoJobService = walletRepoJobService;
            _walletLogger = walletLogger;
        }
        public IServiceProvider Services { get; }

        public async Task<WebApiResponse> ProcessTransactions(List<TransactionLog> pendingRequest)
        {
            long transactionId = 0;
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    foreach (var item in pendingRequest)
                    {
                        _walletLogger.LogRequest($"{"Job Service" + "-" + "Tasks starts to process deliveryday transaction" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " }{DateTime.Now}", false);

                        var requestId = Guid.NewGuid().ToString();
                        var getWalletInfo = await context.MerchantWallet
                           .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                        if (getWalletInfo == null)
                            return null;

                        var getTransInfo = await context.TransactionLog
                            .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);

                        transactionId = getTransInfo.TransactionLogId;

                        getTransInfo.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.WalletFundingProgressFinalDeliveryDay;
                        getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.WalletFundingProgressFinalDeliveryDay;
                       // getTransInfo.TransactionStatus = TransactionJourneyStatusCodes.WalletFundingProgressFinalDeliveryDay;
                        getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.WalletFundingProgressFinalDeliveryDay;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();


                        var walletModel = new WalletTransferRequestDto
                        {
                            CURRENCYCODE = _appSettings.walletcurrencyCode,
                            amt = Convert.ToString(item.TotalAmount),
                            toacct = _appSettings.SterlingWalletPoolAccount,
                            channelID = 1,
                            TransferType = 1,
                            frmacct = getWalletInfo.Mobile,
                            paymentRef = item.PaymentReference,
                            remarks = "Social-Pay Delivery day wallet transfer" + " - " + item.TransactionReference + " - " + item.Category
                        };

                        var walletRequestModel = new DeliveryDayWalletTransferRequestLog
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
                            ChannelMode = WalletTransferMode.MerchantToSocialPay,
                            RequestId = requestId,
                            ClientAuthenticationId = item.ClientAuthenticationId
                        };

                        await context.DeliveryDayWalletTransferRequestLog.AddAsync(walletRequestModel);
                        await context.SaveChangesAsync();

                        var walletResponseModel = new WalletTransferResponse();          

                        var initiateRequest = await _walletRepoJobService.WalletToWalletTransferAsync(walletModel);

                        if (initiateRequest.response == AppResponseCodes.Success)
                        {
                            using(var transaction = await context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    walletResponseModel.message = initiateRequest.message;
                                    walletResponseModel.response = initiateRequest.response;
                                    walletResponseModel.sent = initiateRequest.data.sent;
                                    walletResponseModel.RequestId = walletRequestModel.RequestId;
                                    walletResponseModel.PaymentReference = item.PaymentReference;
                                    walletResponseModel.responsedata = Convert.ToString(initiateRequest.message);
                                    getTransInfo.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.CompletedDeliveryDayWalletFunding;
                                    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.CompletedDeliveryDayWalletFunding;
                                    getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.CompletedDeliveryDayWalletFunding;
                                    getTransInfo.LastDateModified = DateTime.Now;
                                    getTransInfo.WalletFundDate = DateTime.Now;
                                    //getTransInfo.TransactionJourney = OrderStatusCode.CompletedWalletFunding;
                                    context.Update(getTransInfo);
                                    await context.SaveChangesAsync();
                                    await context.WalletTransferResponse.AddAsync(walletResponseModel);
                                    await context.SaveChangesAsync();
                                    await transaction.CommitAsync();
                                    return null;
                                }
                                catch (Exception ex)
                                {
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
                        return null;
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
                _walletLogger.LogRequest($"{"Job Service: An error occured DeliveryDayTransferService." + " | " + transactionId + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);

                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    _walletLogger.LogRequest($"{"Job Service. DeliveryDayTransferService: An error occured. Duplicate transaction reference" + " | " + transactionId + " | " + errorMessage + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
