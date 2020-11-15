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

namespace SocialPay.Job.Repository
{
    public class PendingWalletRequestService
    {
        private readonly WalletRepoJobService _walletRepoService;
        private readonly AppSettings _appSettings;
        public PendingWalletRequestService(IServiceProvider services, WalletRepoJobService walletRepoService,
            IOptions<AppSettings> appSettings)
        {
            Services = services;
            _walletRepoService = walletRepoService;
            _appSettings = appSettings.Value;
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
                        var getTransInfo = await context.TransactionLog
                            .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);

                        getTransInfo.IsWalletQueued = true;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();

                        var getWalletInfo = await context.MerchantWallet
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.CustomerInfo);
                        if(getWalletInfo == null)
                            return null;

                        var walletModel = new WalletTransferRequestDto
                        {
                            CURRENCYCODE = _appSettings.walletcurrencyCode, amt = Convert.ToString(item.TotalAmount),
                            toacct = getWalletInfo.Mobile, channelID = 1, TransferType = 1,
                            frmacct = _appSettings.SterlingWalletPoolAccount, paymentRef = Guid.NewGuid().ToString(),
                            remarks = "Social-Pay wallet transfer" + " - " + item.TransactionReference + " - " + item.Category
                        };

                        var walletRequestModel = new WalletTransferRequestLog
                        {
                            amt = walletModel.amt, channelID = walletModel.channelID, CURRENCYCODE = walletModel.CURRENCYCODE,
                            frmacct = walletModel.frmacct, PaymentReference = walletModel.paymentRef, remarks = walletModel.remarks,
                            toacct = walletModel.toacct, TransactionReference = item.TransactionReference,
                            CustomerTransactionReference = item.CustomerTransactionReference, TransferType = walletModel.TransferType,
                        };

                        await context.WalletTransferRequestLog.AddAsync(walletRequestModel);
                        await context.SaveChangesAsync();

                        var initiateRequest = await _walletRepoService.WalletToWalletTransferAsync(walletModel);
                        if(initiateRequest.response == AppResponseCodes.Success)
                        {
                            getTransInfo.IsWalletCompleted = true;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();
                        }
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
