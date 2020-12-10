﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Job.Repository.Fiorano;
using SocialPay.Job.Repository.InterBankService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowBankTransactions
{
    public class NonEscrowPendingBankTransaction
    {
        private readonly AppSettings _appSettings;
        private readonly FioranoTransferPayWithCardRepository _fioranoTransferRepository;
        private readonly InterBankPendingTransferService _interBankPendingTransferService;
        public NonEscrowPendingBankTransaction(IServiceProvider service, IOptions<AppSettings> appSettings,
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
                         && x.TransactionJourney == TransactionJourneyStatusCodes.WalletTranferCompleted);
                        if (getTransInfo == null)
                            return null;

                        getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.BankTransferProcessing;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();

                        string bankCode = string.Empty;
                        var getBankInfo = await context.MerchantBankInfo
                           .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        if (getBankInfo == null)
                            return null;

                        if (getBankInfo.BankCode == _appSettings.SterlingBankCode)
                        {
                            bankCode = getBankInfo.BankCode;

                            var initiateRequest = await _fioranoTransferRepository
                               .InititiateMerchantCredit(Convert.ToString(getTransInfo.TotalAmount),
                               "Credit Merchant Sterling Acc" + " - " + item.TransactionReference +
                               " - " + item.PaymentReference, item.PaymentReference,
                               getBankInfo.Nuban, true, item.TransactionReference, "Intra-Bank Transfer", 
                               item.PaymentReference);

                            if (initiateRequest.ResponseCode == AppResponseCodes.Success)
                            {
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
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();
                            return null;
                        }
                        
                        getTransInfo.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.BankTransferProcessing;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();
                        await _interBankPendingTransferService.ProcessInterBankTransactions(getBankInfo.Nuban, item.TotalAmount,
                            getBankInfo.BankCode, _appSettings.socialT24AccountNo);

                        getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.TransactionCompleted;
                        getTransInfo.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.TransactionCompleted;
                        getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.TransactionCompleted;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();

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
