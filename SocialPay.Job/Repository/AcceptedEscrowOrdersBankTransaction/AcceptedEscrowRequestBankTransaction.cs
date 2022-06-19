using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.SerilogService.Escrow;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedEscrowOrdersBankTransaction
{
    public class AcceptedEscrowRequestBankTransaction : IAcceptedEscrowRequestBankTransaction
    {
        private readonly AcceptedEscrowRequestPendingBankTransaction _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AcceptedEscrowRequestBankTransaction));
        private readonly EscrowJobLogger _escrowLogger;
        public AcceptedEscrowRequestBankTransaction(AcceptedEscrowRequestPendingBankTransaction transactions, EscrowJobLogger escrowLogger, IServiceProvider services)
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
                _escrowLogger.LogRequest($"{"Job Service to fetch awaiting transactions" + " | " + "AcceptedEscrowRequestPendingBankTransaction" + " | "}{DateTime.Now}", false);
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                   
                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.ActivityStatus == TransactionJourneyStatusCodes.WalletTranferCompleted
                        && x.TransactionStatus == TransactionJourneyStatusCodes.Approved
                        ).ToListAsync();

                    var getEscrowTransactions = pendingTransactions.Where(x => x.Category == MerchantPaymentLinkCategory.Escrow
                   || x.Category == MerchantPaymentLinkCategory.OneOffEscrowLink).ToList();

                    _escrowLogger.LogRequest($"{"Job Service total number of pending transactions" + " | " + pendingTransactions.Count + " | " }{DateTime.Now}", false);
                   
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
                _escrowLogger.LogRequest($"{"Job Service. An error occured while fetching awaiting transactions" + " | " + ex.Message.ToString() + " | " + "AcceptedEscrowRequestPendingBankTransaction" + " | "}{DateTime.Now}", true);
                return "Error";
            }

        }
    }
}
