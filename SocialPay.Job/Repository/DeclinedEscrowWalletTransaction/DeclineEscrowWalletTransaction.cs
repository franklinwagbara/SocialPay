using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeclinedEscrowWalletTransaction
{
    public class DeclineEscrowWalletTransaction : IDeclineEscrowWalletTransaction
    {
        private readonly DeclineEscrowWalletPendingTransaction _transactions;

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(DeclineEscrowWalletTransaction));
        public DeclineEscrowWalletTransaction(DeclineEscrowWalletPendingTransaction transactions, IServiceProvider services)
        {
            Services = services;
            _transactions = transactions;
        }

        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                _log4net.Info("Job Service" + "-" + "DeclineEscrowWalletTransaction" + " | " + DateTime.Now);
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    
                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.ActivityStatus == TransactionJourneyStatusCodes.ItemAccepted
                        && x.TransactionStatus == TransactionJourneyStatusCodes.ItemAccepted
                        ).ToListAsync();
                    
                    var getEscrowTransactions = pendingTransactions.Where(x => x.Category == MerchantPaymentLinkCategory.Escrow
                    || x.Category == MerchantPaymentLinkCategory.OneOffEscrowLink).ToList();

                     _log4net.Info("Job Service" + "-" + "DeclineEscrowWalletTransaction pending transactions" + " | " + pendingTransactions.Count + " | " + DateTime.Now);
                   
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
                  _log4net.Error("Job Service: Error occured while fetching awaiting transactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                return "Error";
            }

        }
    }
}
