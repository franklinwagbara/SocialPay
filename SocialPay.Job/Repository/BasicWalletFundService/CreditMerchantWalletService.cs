using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.BasicWalletFundService
{
    public class CreditMerchantWalletService : ICreditMerchantWalletService
    {
        private readonly CreditMerchantWalletTransactions _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(CreditMerchantWalletService));

        public CreditMerchantWalletService(IServiceProvider services, CreditMerchantWalletTransactions transactions)
        {
            Services = services;
            _transactions = transactions;
        }
        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                _log4net.Info("Job Service" + "-" + "CreditMerchantWalletService" + " | " + DateTime.Now);
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.OrderStatus == TransactionJourneyStatusCodes.Pending).ToListAsync();
                     _log4net.Info("Job Service" + "-" + "CreditMerchantWalletService pending transactions" + " | " + pendingTransactions.Count + " | " + DateTime.Now);
                    if (pendingTransactions.Count == 0)
                        return "No record";
                    await _transactions.ProcessTransactions(pendingTransactions);
                }

                Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";
            }
            catch (Exception ex)
            {
                _log4net.Error("Job Service" + "-" + "Error occured" + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                return "Error";
            }

        }

    }
}
