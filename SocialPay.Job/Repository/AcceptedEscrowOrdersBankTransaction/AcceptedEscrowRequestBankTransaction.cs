using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedEscrowOrdersBankTransaction
{
    public class AcceptedEscrowRequestBankTransaction : IAcceptedEscrowRequestBankTransaction
    {
        private readonly AcceptedEscrowRequestPendingBankTransaction _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AcceptedEscrowRequestBankTransaction));

        public AcceptedEscrowRequestBankTransaction(AcceptedEscrowRequestPendingBankTransaction transactions, IServiceProvider services)
        {
            Services = services;
            _transactions = transactions;
        }

        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                 _log4net.Info("Job Service to fetch awaiting transactions" + " | " + "AcceptedEscrowRequestPendingBankTransaction" + " | "+ DateTime.Now);
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    DateTime nextDay = DateTime.Now.Date.AddDays(1);
                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.ActivityStatus == TransactionJourneyStatusCodes.WalletTranferCompleted
                        && x.TransactionStatus == TransactionJourneyStatusCodes.Approved
                        ).ToListAsync();
                     _log4net.Info("Job Service total number of pending transactions" + " | " + pendingTransactions.Count + " | " + DateTime.Now);
                    if (pendingTransactions.Count == 0)
                        return "No record";
                    await _transactions.ProcessTransactions(pendingTransactions);
                    //return "No record";
                }

                Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";
            }
            catch (Exception ex)
            {
                _log4net.Error("Job Service. An error occured while fetching awaiting transactions" + " | " + ex.Message.ToString() + " | " + "AcceptedEscrowRequestPendingBankTransaction" +" | "+DateTime.Now);
                return "Error";
            }

        }
    }
}
