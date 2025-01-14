﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.SerilogService.NonEscrowJob;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowOtherWalletTransaction
{
    public class NonEscrowOtherWalletTransaction : INonEscrowOtherWalletTransaction
    {
        private readonly NonEscrowOtherWalletPendingTransaction _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NonEscrowOtherWalletTransaction));
        private readonly NonEscrowJobLogger _nonescrowLogger;
        public NonEscrowOtherWalletTransaction(NonEscrowOtherWalletPendingTransaction transactions, IServiceProvider services, NonEscrowJobLogger nonescrowLogger)
        {
            Services = services;
            _transactions = transactions;
            _nonescrowLogger = nonescrowLogger;
        }

        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                _nonescrowLogger.LogRequest($"{"Job Service" + "-" + "to fetch awaiting transactions for NonEscrowWalletTransaction" + " | "}{DateTime.Now}", false);

                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    DateTime nextDay = DateTime.Now.Date.AddDays(1);

                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.TransactionJourney == TransactionJourneyStatusCodes.FirstWalletFundingWasSuccessul
                         && x.PaymentChannel != PaymentChannel.Card
                        ).Take(1).ToListAsync();

                    var getNonEscrowTransactions = pendingTransactions.Where(x => x.Category == MerchantPaymentLinkCategory.Basic
                    || x.Category == MerchantPaymentLinkCategory.OneOffBasicLink).ToList();
                    _nonescrowLogger.LogRequest($"{"Job Service: NonEscrowWalletTransaction. Total number of pending transactions" + " | " + pendingTransactions.Count + " | "}{DateTime.Now}", false);

                    if (getNonEscrowTransactions.Count == 0)
                        return "No record";
                    
                    await _transactions.ProcessTransactions(getNonEscrowTransactions);
                }

                Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";
            }
            catch (Exception ex)
            {
                _nonescrowLogger.LogRequest($"{"Job Service. NonEscrowWalletTransaction" + "Error occured" + " | " + ex.Message.ToString() + " | " }{DateTime.Now}", true);

                return "Error";
            }

        }
    }
}
