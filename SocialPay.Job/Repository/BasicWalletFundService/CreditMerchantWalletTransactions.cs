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
using SocialPay.Helper.SerilogService.WalletJob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.BasicWalletFundService
{
    public class CreditMerchantWalletTransactions
    {
        private readonly AppSettings _appSettings;
        private readonly WalletRepoJobService _walletRepoJobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(CreditMerchantWalletTransactions));
        private readonly WalletJobLogger _walletLogger;

        public CreditMerchantWalletTransactions(IServiceProvider service, IOptions<AppSettings> appSettings,
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
            long transactionLogid = 0;
            try
            {                
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    foreach (var item in pendingRequest)
                    {
                        _walletLogger.LogRequest($"{"Job Service" + "-" + "Credit merchant wallet" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " }{DateTime.Now}", false);
                        var requestId = $"{"So-Pay-"}{Guid.NewGuid().ToString().Substring(0, 22)}";
                        var getTransInfo = await context.TransactionLog
                           .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId
                           && x.OrderStatus == TransactionJourneyStatusCodes.Pending);

                        if (getTransInfo == null)
                            return null;

                        getTransInfo.OrderStatus = TransactionJourneyStatusCodes.WalletFundingProgress;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);

                        await context.SaveChangesAsync();

                        transactionLogid = getTransInfo.TransactionLogId;

                        var getWalletInfo = await context.MerchantWallet
                           .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                       
                        if (getWalletInfo == null)
                        {
                            _walletLogger.LogRequest($"{"Job Service" + "-" + "Credit merchant wallet. Bank info is null" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);
                            return null;
                        }

                        var walletModel = new WalletTransferRequestDto
                        {
                            CURRENCYCODE = _appSettings.walletcurrencyCode,
                            amt = Convert.ToString(item.TotalAmount),
                            toacct = getWalletInfo.Mobile,
                            channelID = 1,
                            TransferType = 1,
                            frmacct = _appSettings.SterlingWalletPoolAccount,
                            paymentRef = requestId,
                            remarks = "Social-Pay Core pool to merchant wallet" + " - " + item.PaymentReference + " - " + requestId
                        };









                        var walletRequestModel = new DefaultWalletTransferRequestLog
                        {
                            amt = Convert.ToDecimal(walletModel.amt),
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
                            RequestId = requestId,
                        };

                        await context.DefaultWalletTransferRequestLog.AddAsync(walletRequestModel);
                        await context.SaveChangesAsync();

                        _walletLogger.LogRequest($"{"Job Service" + "-" + "DefaultWalletTransferRequestLog merchant wallet request was successfully logged" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

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

                                    //Lock fund here



                                    _walletLogger.LogRequest($"{"Job Service" + "-" + "Credit merchant wallet saved and updated" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);
                                }
                                catch (Exception ex)
                                {
                                    _walletLogger.LogRequest($"{"Job Service" + "-" + "Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);
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
            catch (SqlException ex)
            {
                _walletLogger.LogRequest($"{"Job Service" + "-" + "Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

            catch (Exception ex)
            {
                _walletLogger.LogRequest($"{"Job Service" + "-" + "Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);
                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    _walletLogger.LogRequest($"{"Job Service" + "-" + "Error occured. Duplicate transaction" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
