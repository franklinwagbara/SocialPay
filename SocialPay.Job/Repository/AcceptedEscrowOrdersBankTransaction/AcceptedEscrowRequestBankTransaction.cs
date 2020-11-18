using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedEscrowOrdersBankTransaction
{
    public class AcceptedEscrowRequestBankTransaction : IAcceptedEscrowRequestBankTransaction
    {
        private readonly AcceptedEscrowRequestPendingBankTransaction _transactions;
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
                //  _log4net.Info("Tasks starts to fetch awaiting transactions" + " | " + DateTime.Now);
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    DateTime nextDay = DateTime.Now.Date.AddDays(1);
                    var pendingTransactions = await context.TransactionLog
                        //.Where(x => x.TransactionJourney == TransactionJourneyStatusCodes.FioranoFirstFundingCompleted
                        //|| x.TransactionJourney == TransactionJourneyStatusCodes.AwaitingCustomerFeedBack
                        ////|| x.TransactionJourney == TransactionJourneyStatusCodes.CompletedDeliveryDayWalletFunding
                        //&& x.TransactionStatus == OrderStatusCode.Approved
                        .Where(x => x.TransactionStatus == TransactionJourneyStatusCodes.WalletTranferCompleted
                        ).ToListAsync();
                    // _log4net.Info("Total number of pending transactions" + " | " + pendingTransactions.Count + " | " + DateTime.Now);
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
                //  _log4net.Error("An error occured while fetching awaiting transactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                return "Error";
            }

        }
    }
}
