﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.SerilogService.NonEscrowJob;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowBankTransactions
{
    public class NonEscrowBankTransaction : INonEscrowBankTransaction
    {
        private readonly NonEscrowPendingBankTransaction _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NonEscrowBankTransaction));
        private readonly NonEscrowJobLogger _nonescrowLogger;
        public NonEscrowBankTransaction(NonEscrowPendingBankTransaction transactions, IServiceProvider services, NonEscrowJobLogger nonescrowLogger)
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
                _nonescrowLogger.LogRequest($"{"Job Service" + "-" + "NonEscrowBankTransaction request" + " | "}{DateTime.Now}", false);

                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
          
                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.TransactionJourney == 
                        TransactionJourneyStatusCodes.WalletTranferCompleted).Take(1).ToListAsync();

                    var getNonEscrowTransactions = pendingTransactions.Where(x => x.LinkCategory == MerchantPaymentLinkCategory.Basic
                     || x.LinkCategory == MerchantPaymentLinkCategory.OneOffBasicLink).ToList();
                   
                    _nonescrowLogger.LogRequest($"{"Job Service: Total number of pending transactions" + " | " + pendingTransactions.Count + " | "}{DateTime.Now}", false);

                    if (getNonEscrowTransactions.Count == 0)
                        return "No record";
                   
                    await _transactions.ProcessTransactions(getNonEscrowTransactions);
                }

                Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";
            }
            catch (Exception ex)
            {
                _nonescrowLogger.LogRequest($"{"Job Service: An error occured while fetching transactions" + " | " + ex.Message.ToString() + " | " }{DateTime.Now}", true);

                return "Error";
            }

        }
    }
}
