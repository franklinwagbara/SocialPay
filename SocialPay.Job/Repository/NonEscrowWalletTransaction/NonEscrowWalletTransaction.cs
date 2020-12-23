using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowWalletTransaction
{
    public class NonEscrowWalletTransaction : INonEscrowWalletTransaction
    {
        private readonly NonEscrowWalletPendingTransaction _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NonEscrowWalletTransaction));

        public NonEscrowWalletTransaction(NonEscrowWalletPendingTransaction transactions, IServiceProvider services)
        {
            Services = services;
            _transactions = transactions;
        }

        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                _log4net.Info("Job Service" + "-" + "to fetch awaiting transactions" + " | " + DateTime.Now);
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    DateTime nextDay = DateTime.Now.Date.AddDays(1);

                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.TransactionJourney == TransactionJourneyStatusCodes.FioranoFirstFundingCompleted
                        || x.TransactionJourney == TransactionJourneyStatusCodes.FirstWalletFundingWasSuccessul
                        ).ToListAsync();

                    var getNonEscrowTransactions = pendingTransactions.Where(x => x.Category == MerchantPaymentLinkCategory.Basic
                    || x.Category == MerchantPaymentLinkCategory.OneOffBasicLink).ToList();
                     _log4net.Info("Job Service. Total number of pending transactions" + " | " + pendingTransactions.Count + " | " + DateTime.Now);
                  
                    if (getNonEscrowTransactions.Count == 0)
                        return "No record";
                    
                    await _transactions.ProcessTransactions(getNonEscrowTransactions);
                }

                Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";
            }
            catch (Exception ex)
            {
                 _log4net.Error("Job Service." + "Error occured" + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                return "Error";
            }

        }
    }
}
