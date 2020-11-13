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

namespace SocialPay.Job.Repository.IntraBankService
{
    public class IntraBankPendingTransactions
    {
        private readonly AppSettings _appSettings;
        private readonly FioranoTransferPayWithCardRepository _fioranoTransferRepository;
        private readonly InterBankPendingTransferService _interBankPendingTransferService;
 
        public IntraBankPendingTransactions(IServiceProvider service, IOptions<AppSettings> appSettings,
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
                        string bankCode = string.Empty;
                        var getBankInfo = await context.MerchantBankInfo
                           .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        if (getBankInfo == null)
                            return null;

                        if(getBankInfo.BankCode == _appSettings.SterlingBankCode)
                        {
                            bankCode = getBankInfo.BankCode;
                            var getTransInfo = await context.TransactionLog
                           .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);

                            getTransInfo.DeliveryDayTransferStatus = OrderStatusCode.WalletFundingProgress;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();

                            var initiateRequest = await _fioranoTransferRepository
                               .InititiateDebit(Convert.ToString(getTransInfo.TotalAmount),
                               "Card-Payment" + " - " + item.TransactionReference +
                               " - " + item.CustomerTransactionReference, item.TransactionReference,
                               getBankInfo.Nuban, true);

                            if (initiateRequest.ResponseCode == AppResponseCodes.Success)
                            {
                                getTransInfo.DeliveryDayTransferStatus = OrderStatusCode.CompletedDirectFundTransfer;
                                getTransInfo.LastDateModified = DateTime.Now;
                                context.Update(getTransInfo);
                                await context.SaveChangesAsync();
                                return null;
                            }

                            getTransInfo.DeliveryDayTransferStatus = OrderStatusCode.Failed;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();
                            //return null;
                        }

                       // await _interBankPendingTransferService.ProcessTransactions

                       //Other banks transfer
                      //  return null;
                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
