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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.BasicWalletFundService
{
    public class CreditMerchantWalletTransactions
    {
        private readonly AppSettings _appSettings;
        private readonly WalletRepoJobService _walletRepoJobService;

     
        public CreditMerchantWalletTransactions(IServiceProvider service, IOptions<AppSettings> appSettings,
         WalletRepoJobService walletRepoJobService)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _walletRepoJobService = walletRepoJobService;
        }
        public IServiceProvider Services { get; }

        public async Task<WebApiResponse> ProcessTransactions(List<TransactionLog> pendingRequest)
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
                           && x.OrderStatus == TransactionJourneyStatusCodes.Pending);
                        if (getTransInfo == null)
                            return null;
                        getTransInfo.OrderStatus = TransactionJourneyStatusCodes.WalletFundingProgress;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();

                        var getWalletInfo = await context.MerchantWallet
                           .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        if (getWalletInfo == null)
                            return null;

                        var walletModel = new WalletTransferRequestDto
                        {
                            CURRENCYCODE = _appSettings.walletcurrencyCode,
                            amt = Convert.ToString(item.TotalAmount),
                            toacct = getWalletInfo.Mobile,
                            channelID = 1,
                            TransferType = 1,
                            frmacct = _appSettings.SterlingWalletPoolAccount,
                            paymentRef = item.PaymentReference,
                            remarks = "Social-Pay Core pool to merchant wallet" + " - " + item.PaymentReference + " - " + item.PaymentChannel
                        };

                        var walletRequestModel = new DefaultWalletTransferRequestLog
                        {
                            amt = walletModel.amt,
                            channelID = walletModel.channelID,
                            CURRENCYCODE = walletModel.CURRENCYCODE,
                            frmacct = walletModel.frmacct,
                            PaymentReference = item.PaymentReference,
                            remarks = walletModel.remarks,
                            toacct = walletModel.toacct,
                            TransactionReference = item.TransactionReference,
                            CustomerTransactionReference = item.CustomerTransactionReference,
                            TransferType = walletModel.TransferType,
                            ChannelMode = WalletTransferMode.SocialPayToMerchant,
                            ClientAuthenticationId = item.ClientAuthenticationId,
                            RequestId = requestId
                        };

                        await context.DefaultWalletTransferRequestLog.AddAsync(walletRequestModel);
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
                                        PaymentReference = walletRequestModel.PaymentReference,
                                        responsedata = Convert.ToString(initiateRequest.responsedata),
                                    };
                                    //if(getTransInfo.Category == MerchantPaymentLinkCategory.Escrow ||
                                    //    getTransInfo.Category == MerchantPaymentLinkCategory.OneOffEscrowLink)
                                    //{
                                    //    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.AwaitingCustomerFeedBack;
                                    //}

                                    //if (getTransInfo.PaymentChannel == PaymentChannel.Card)
                                    //{
                                    //    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.AwaitingCustomerFeedBack;
                                    //}
                                    getTransInfo.OrderStatus = TransactionJourneyStatusCodes.CompletedWalletFunding;
                                    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.FirstWalletFundingWasSuccessul;
                                    getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.FirstWalletFundingWasSuccessul;
                                    getTransInfo.LastDateModified = DateTime.Now;
                                    getTransInfo.WalletFundDate = DateTime.Now;
                                    context.Update(getTransInfo);
                                    await context.SaveChangesAsync();
                                    await context.WalletTransferResponse.AddAsync(walletResponse);
                                    await context.SaveChangesAsync();
                                    await transaction.CommitAsync();
                                }
                                catch (Exception ex)
                                {
                                    await transaction.RollbackAsync();
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
