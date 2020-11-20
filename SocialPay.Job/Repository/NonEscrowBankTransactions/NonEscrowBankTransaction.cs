using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowBankTransactions
{
    public class NonEscrowBankTransaction : INonEscrowBankTransaction
    {
        private readonly NonEscrowPendingBankTransaction _transactions;
        public NonEscrowBankTransaction(NonEscrowPendingBankTransaction transactions, IServiceProvider services)
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
                        .Where(x => x.TransactionJourney == 
                        TransactionJourneyStatusCodes.WalletTranferCompleted).Take(1).ToListAsync();
                    var getNonEscrowTransactions = pendingTransactions.Where(x => x.LinkCategory == MerchantPaymentLinkCategory.Basic
                     || x.LinkCategory == MerchantPaymentLinkCategory.OneOffBasicLink).ToList();
                    // _log4net.Info("Total number of pending transactions" + " | " + pendingTransactions.Count + " | " + DateTime.Now);
                    if (getNonEscrowTransactions.Count == 0)
                        return "No record";
                    await _transactions.ProcessTransactions(getNonEscrowTransactions);
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
