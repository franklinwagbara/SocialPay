using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Job.Repository.Fiorano;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.PayWithCard
{
    public class PendingPayWithCardTransaction
    {
        private readonly FioranoTransferPayWithCardRepository _fioranoTransferRepository;
        public PendingPayWithCardTransaction(IServiceProvider services, 
            FioranoTransferPayWithCardRepository fioranoTransferRepository)
        {
            Services = services;
            _fioranoTransferRepository = fioranoTransferRepository;
        }
        public IServiceProvider Services { get; }

        public async Task<WebApiResponse> InitiateTransactions(List<TransactionLog> pendingRequest)
        {
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    foreach (var item in pendingRequest)
                    {
                        var getTransInfo = await context.TransactionLog
                            .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);
                        getTransInfo.IsQueuedPayWithCard = true;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();

                        var getWalletInfo = await context.MerchantWallet
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        if (getWalletInfo == null)
                            return null;

                        var initiateRequest = await _fioranoTransferRepository
                            .InititiateDebit(Convert.ToString(getTransInfo.TotalAmount), 
                            "Card-Payment" + " - " +item.TransactionReference + " - "+ item.CustomerTransactionReference);
                        if (initiateRequest.ResponseCode == AppResponseCodes.Success)
                        {
                            getTransInfo.IsApproved = true;
                            getTransInfo.IsCompletedPayWithCard = true;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();
                            return null;
                        }

                        getTransInfo.IsQueuedPayWithCard = false;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();
                        return null;
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
