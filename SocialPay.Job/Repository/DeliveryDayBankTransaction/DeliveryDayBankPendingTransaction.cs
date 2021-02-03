using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.Validations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Job.Repository.Fiorano;
using SocialPay.Job.Repository.InterBankService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeliveryDayBankTransaction
{
    public class DeliveryDayBankPendingTransaction
    {
        private readonly AppSettings _appSettings;
        private readonly BankServiceRepositoryJobService _bankServiceRepositoryJobService;
        private readonly DeliveryDayFioranoTransferRepository _fioranoTransferRepository;
        private readonly DeliveryDayInterBankPendingTransferService _interBankPendingTransferService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(DeliveryDayBankPendingTransaction));

        public DeliveryDayBankPendingTransaction(IServiceProvider service, IOptions<AppSettings> appSettings,
             DeliveryDayFioranoTransferRepository fioranoTransferRepository,
         DeliveryDayInterBankPendingTransferService interBankPendingTransferService,
         BankServiceRepositoryJobService bankServiceRepositoryJobService)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _fioranoTransferRepository = fioranoTransferRepository;
            _interBankPendingTransferService = interBankPendingTransferService;
            _bankServiceRepositoryJobService = bankServiceRepositoryJobService;
        }
        public IServiceProvider Services { get; }


        public async Task<WebApiResponse> ProcessTransactions(List<TransactionLog> pendingRequest)
        {
            long transactionLogId = 0;
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    foreach (var item in pendingRequest)
                    {
                        _log4net.Info("Job Service. Task starts to initiate DeliveryDayBankPendingTransaction" + " | " + item.PaymentReference + " | " +  " | " + DateTime.Now);


                        var validateNuban = await _bankServiceRepositoryJobService.GetAccountFullInfoAsync(_appSettings.socialT24AccountNo, item.TotalAmount);

                        if (validateNuban.ResponseCode == AppResponseCodes.Success)
                        {
                            var requestId = Guid.NewGuid().ToString();
                            var getTransInfo = await context.TransactionLog
                             .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);

                            if (getTransInfo == null)
                                return null;

                            transactionLogId = getTransInfo.TransactionLogId;

                            string bankCode = string.Empty;
                            var getBankInfo = await context.MerchantBankInfo
                               .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                            if (getBankInfo == null)
                                return null;
                            //getBankInfo.BankCode = "000014";

                            if (getBankInfo.BankCode == _appSettings.SterlingBankCode)
                            {
                                bankCode = getBankInfo.BankCode;

                                getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.BankTransferProcessing;
                                getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.BankTransferProcessing;
                                getTransInfo.LastDateModified = DateTime.Now;
                                context.Update(getTransInfo);
                                await context.SaveChangesAsync();

                                var initiateRequest = await _fioranoTransferRepository
                                   .InititiateDebit(Convert.ToString(getTransInfo.TotalAmount),
                                   "Credit Merchant Sterling Acc" + " - " + item.TransactionReference +
                                   " - " + item.PaymentReference, item.TransactionReference,
                                   getBankInfo.Nuban, item.PaymentChannel, "Intra-Bank Transfer", item.PaymentReference,
                                   getTransInfo.TransactionLogId);

                                if (initiateRequest.ResponseCode == AppResponseCodes.Success)
                                {
                                    getTransInfo.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.TransactionCompleted;
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

                            var initiateInterBankRequest = await _interBankPendingTransferService.ProcessInterBankTransactions(getBankInfo.Nuban, item.TotalAmount,
                                getBankInfo.BankCode, _appSettings.socialT24AccountNo, item.ClientAuthenticationId,
                                item.PaymentReference, item.TransactionReference);

                            if (initiateInterBankRequest.ResponseCode == AppResponseCodes.Success)
                            {
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

                        else
                        {
                            var failedResponse = new FailedTransactions
                            {
                                CustomerTransactionReference = item.CustomerTransactionReference,
                                Message = "Name enquiry issues" + "-" + validateNuban.UsableBal + "-" + item.TotalAmount + "-" + validateNuban.ResponseCode + "-" + item.PaymentReference,
                                TransactionReference = item.TransactionReference
                            };
                            await context.FailedTransactions.AddAsync(failedResponse);
                            await context.SaveChangesAsync();
                        }

                        //Other banks transfer
                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Job Service An error occured. Base error" + " | " + transactionLogId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return null;
            }
        }
    }
}
