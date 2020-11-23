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
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeliveryDayBankTransaction
{
    public class DeliveryDayBankPendingTransaction
    {
        private readonly AppSettings _appSettings;
        private readonly FioranoTransferPayWithCardRepository _fioranoTransferRepository;
        private readonly InterBankPendingTransferService _interBankPendingTransferService;
        public DeliveryDayBankPendingTransaction(IServiceProvider service, IOptions<AppSettings> appSettings,
             FioranoTransferPayWithCardRepository fioranoTransferRepository,
         InterBankPendingTransferService interBankPendingTransferService)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _fioranoTransferRepository = fioranoTransferRepository;
            _interBankPendingTransferService = interBankPendingTransferService;
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
                         && x.ActivityStatus == TransactionJourneyStatusCodes.WalletTranferCompleted
                         && x.TransactionStatus == TransactionJourneyStatusCodes.Approved);
                        if (getTransInfo == null)
                            return null;
                        string bankCode = string.Empty;
                        var getBankInfo = await context.MerchantBankInfo
                           .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        if (getBankInfo == null)
                            return null;

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
                               " - " + item.PaymentReference, item.PaymentReference,
                               getBankInfo.Nuban, true, item.PaymentChannel, "Intra-Bank Transfer", requestId);

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
                        ////getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.BankTransferProcessing;
                        ////getTransInfo.LastDateModified = DateTime.Now;
                        ////context.Update(getTransInfo);
                        ////await context.SaveChangesAsync();
                        ////var processIntebankTransction = await _interBankPendingTransferService.ProcessInterBankTransactions(getBankInfo.Nuban, item.TotalAmount,
                        ////    getBankInfo.BankCode, _appSettings.socialT24AccountNo);

                        ////getTransInfo.TransactionStatus = OrderStatusCode.TransactionCompleted;
                        ////getTransInfo.DeliveryDayTransferStatus = OrderStatusCode.TransactionCompleted;
                        ////getTransInfo.TransactionJourney = OrderStatusCode.TransactionCompleted;
                        ////getTransInfo.LastDateModified = DateTime.Now;
                        ////context.Update(getTransInfo);
                        ////await context.SaveChangesAsync();

                        //Other banks transfer
                        return null;
                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
