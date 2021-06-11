using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.PayWithCard
{
    public class PayWithCardTransaction : IPayWithCardTransaction
    {
        private readonly PendingPayWithCardTransaction _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(PayWithCardTransaction));

        public PayWithCardTransaction(IServiceProvider services, PendingPayWithCardTransaction transactions)
        {
            Services = services;
            _transactions = transactions;
        }
        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                _log4net.Info("Job Service" + "-" + "PendingPayWithCardTransaction transactions" + " | " +  DateTime.Now);
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.OrderStatus == TransactionJourneyStatusCodes.CompletedWalletFunding 
                        && x.TransactionJourney == TransactionJourneyStatusCodes.FirstWalletFundingWasSuccessul
                        && x.PaymentChannel == PaymentChannel.Card).Take(4).ToListAsync();

                    _log4net.Info("Job Service: Total number of pending cards transactions" + " | " + pendingTransactions.Count + " | " + DateTime.Now);
                    
                    if (pendingTransactions.Count == 0)
                        return "No record";
                    
                    await _transactions.InitiateTransactions(pendingTransactions);
                }

                Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";
            }
            catch (Exception ex)
            {
                _log4net.Error("Job Service: An error occured while fetching transactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                return "Error";
            }

        }

    }
}
