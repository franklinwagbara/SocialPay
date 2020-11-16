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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeliveryDayMerchantWalletTransaction
{
    public class DeliveryDayTransferService
    {
        private readonly AppSettings _appSettings;
        private readonly WalletRepoJobService _walletRepoJobService;


        public DeliveryDayTransferService(IServiceProvider service, IOptions<AppSettings> appSettings,
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
                        var getWalletInfo = await context.MerchantWallet
                           .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        if (getWalletInfo == null)
                            return null;

                        var getTransInfo = await context.TransactionLog
                            .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);

                        getTransInfo.DeliveryDayTransferStatus = OrderStatusCode.WalletFundingProgress;
                        getTransInfo.StatusJourney = OrderStatusCode.WalletFundingProgress;
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
                            paymentRef = Guid.NewGuid().ToString(),
                            remarks = "Social-Pay Delivery day wallet transfer" + " - " + item.TransactionReference + " - " + item.Category
                        };

                        var walletRequestModel = new WalletTransferRequestLog
                        {
                            amt = walletModel.amt,
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
                            RequestId = requestId
                        };

                        await context.WalletTransferRequestLog.AddAsync(walletRequestModel);
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
                                    walletResponseModel.responsedata = Convert.ToString(initiateRequest.responsedata);
                                    getTransInfo.DeliveryDayTransferStatus = OrderStatusCode.CompletedWalletFunding;
                                    getTransInfo.LastDateModified = DateTime.Now;
                                    getTransInfo.WalletFundDate = DateTime.Now;
                                    getTransInfo.StatusJourney = OrderStatusCode.CompletedWalletFunding;
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
