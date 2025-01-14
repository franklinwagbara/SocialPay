﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.SerilogService.Escrow;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedEscrowOrdersWalletTransaction
{
    public class AcceptedEscrowOrders : IAcceptedEscrowOrders
    {
        private readonly AcceptedEscrowOrderTransactions _transactions;

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AcceptedEscrowOrders));
        private readonly EscrowJobLogger _escrowLogger;
        public AcceptedEscrowOrders(AcceptedEscrowOrderTransactions transactions, IServiceProvider services, EscrowJobLogger escrowLogger)
        {
            Services = services;
            _transactions = transactions;
            _escrowLogger = escrowLogger;
        }

        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                _escrowLogger.LogRequest($"{"Job Service" + "-" + "AcceptedEscrowOrders" + " | " }{DateTime.Now}", false);

                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>(); 

                    var pendingTransactions = await context.TransactionLog
                       
                        .Where(x=>x.ActivityStatus == TransactionJourneyStatusCodes.Approved
                        ).ToListAsync();

                    var getEscrowTransactions = pendingTransactions.Where(x => x.Category == MerchantPaymentLinkCategory.Escrow
                    || x.Category == MerchantPaymentLinkCategory.OneOffEscrowLink).ToList();
                    _escrowLogger.LogRequest($"{"Job Service" + "-" + "AcceptedEscrowOrders pending transactions" + " | " + getEscrowTransactions.Count + " | " }{DateTime.Now}", false);
                  
                    if (getEscrowTransactions.Count == 0)
                        return "No record";
                    
                    await _transactions.ProcessTransactions(getEscrowTransactions);
                    //return "No record";
                }

                Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";
            }
            catch (Exception ex)
            {
                _escrowLogger.LogRequest($"{"Job Service. AcceptedEscrowOrders: Error occured while fetching awaiting transactions" + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);
                return "Error";
            }

        }
    }
}
