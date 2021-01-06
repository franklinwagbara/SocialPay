using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Job.Repository.Fiorano;
using SocialPay.Job.Repository.InterBankService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedEscrowOrdersBankTransaction
{
    public class AcceptedEscrowRequestPendingBankTransaction
    {
        private readonly AppSettings _appSettings;
        private readonly FioranoAcceptedEscrowRepository _fioranoTransferRepository;
        private readonly AcceptedEscrowInterBankPendingTransferService _interBankPendingTransferService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AcceptedEscrowRequestPendingBankTransaction));

        public AcceptedEscrowRequestPendingBankTransaction(IServiceProvider service, IOptions<AppSettings> appSettings,
             FioranoAcceptedEscrowRepository fioranoTransferRepository,
         AcceptedEscrowInterBankPendingTransferService interBankPendingTransferService)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _fioranoTransferRepository = fioranoTransferRepository;
            _interBankPendingTransferService = interBankPendingTransferService;
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
                        _log4net.Info("Job Service" + "-" + "Accepted escrow bank transaction" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                        var requestId = Guid.NewGuid().ToString();
                        var getTransInfo = await context.TransactionLog
                         .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId
                         && x.ActivityStatus == TransactionJourneyStatusCodes.WalletTranferCompleted
                         && x.TransactionStatus == TransactionJourneyStatusCodes.Approved);

                        if (getTransInfo == null)
                            return null;

                        transactionLogid = item.TransactionLogId;

                        string bankCode = string.Empty;
                        var getBankInfo = await context.MerchantBankInfo
                           .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        if (getBankInfo == null)
                            return null;

                        if (getBankInfo.BankCode == _appSettings.SterlingBankCode)
                        {
                            _log4net.Info("Job Service" + "-" + "Processing intra bank transaction" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                            bankCode = getBankInfo.BankCode;

                            getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.BankTransferProcessing;
                            getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.BankTransferProcessing;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();

                            var initiateRequest = await _fioranoTransferRepository
                               .InititiateEscrowAcceptedRequest(Convert.ToString(getTransInfo.TotalAmount),
                               "Credit Merchant Sterling Acc" + " - " + item.TransactionReference +
                               " - " + item.TransactionReference, item.TransactionReference, getBankInfo.Nuban, item.PaymentChannel,
                               "Intra-Bank Transfer", item.PaymentReference);

                            if (initiateRequest.ResponseCode == AppResponseCodes.Success)
                            {
                                _log4net.Info("Job Service" + "-" + "Accepted escrow bank request was successful" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                                getTransInfo.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.CompletedDirectFundTransfer;
                                getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.TransactionCompleted;
                                getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.TransactionCompleted;
                                getTransInfo.LastDateModified = DateTime.Now;
                                context.Update(getTransInfo);
                                await context.SaveChangesAsync();
                                return null;
                            }

                            getTransInfo.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.TransactionFailed;
                            getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.TransactionFailed;
                            getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.TransactionFailed;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();
                            return null;
                        }

                        _log4net.Info("Job Service" + "-" + "Accepted escrow inter bank transaction" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                        var initiateInterBankRequest = await _interBankPendingTransferService.ProcessInterBankTransactions(getBankInfo.Nuban, item.TotalAmount,
                            getBankInfo.BankCode, _appSettings.socialT24AccountNo, item.ClientAuthenticationId,
                            item.PaymentReference, item.TransactionReference);

                        if (initiateInterBankRequest.ResponseCode == AppResponseCodes.Success)
                        {
                            _log4net.Info("Job Service" + "-" + "Accepted escrow inter bank request was successful" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                            getTransInfo.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.CompletedDirectFundTransfer;
                            getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.TransactionCompleted;
                            getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.TransactionCompleted;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();
                            return null;
                        }

                        var failedResponse = new FailedTransactions
                        {
                            CustomerTransactionReference = item.CustomerTransactionReference,
                            Message = initiateInterBankRequest.Data.ToString(),
                            TransactionReference = item.TransactionReference
                        };
                        await context.FailedTransactions.AddAsync(failedResponse);
                        await context.SaveChangesAsync();
                        return null;
                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
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

                    _log4net.Error("An error occured. Duplicate transaction reference" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
