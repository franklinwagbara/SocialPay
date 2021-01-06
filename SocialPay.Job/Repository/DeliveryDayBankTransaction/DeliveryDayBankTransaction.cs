using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeliveryDayBankTransaction
{
    public class DeliveryDayBankTransaction : IDeliveryDayBankTransaction
    {
        private readonly DeliveryDayBankPendingTransaction _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(DeliveryDayBankTransaction));

        public DeliveryDayBankTransaction(DeliveryDayBankPendingTransaction transactions, IServiceProvider services)
        {
            Services = services;
            _transactions = transactions;
        }

        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                _log4net.Info("Job Service" + "-" + "DeliveryDayBankTransaction" + " | " + DateTime.Now);

                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                  
                    var pendingTransactions = await context.TransactionLog                       
                        .Where(x => x.ActivityStatus == TransactionJourneyStatusCodes.CompletedDeliveryDayWalletFunding
                        && x.TransactionJourney == TransactionJourneyStatusCodes.CompletedDeliveryDayWalletFunding
                        ).ToListAsync();
                  
                    var getEscrowTransactions = pendingTransactions.Where(x => x.Category == MerchantPaymentLinkCategory.Escrow
                    || x.Category == MerchantPaymentLinkCategory.OneOffEscrowLink).ToList();
                    _log4net.Info("Job Service" + "-" + "DeliveryDayBankTransaction pending transactions" + " | " + pendingTransactions.Count + " | " + DateTime.Now);

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
                _log4net.Error("Job Service: DeliveryDayBankTransaction" + "-" + "Error occured" + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                return "Error";
            }

        }
    }
}
