using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.SerilogService.NonEscrowJob;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowCardWalletTransaction
{
    public class NonEscrowCardWalletTransaction : INonEscrowCardWalletTransaction
    {
        private readonly NonEscrowCardWalletPendingTransaction _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NonEscrowCardWalletTransaction));
        private readonly NonEscrowJobLogger _nonescrowLogger;
        public NonEscrowCardWalletTransaction(NonEscrowCardWalletPendingTransaction transactions, IServiceProvider services, NonEscrowJobLogger nonescrowLogger)
        {
            Services = services;
            _transactions = transactions;
            _nonescrowLogger = nonescrowLogger;
        }

        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                _nonescrowLogger.LogRequest($"{"Job Service" + "-" + "to fetch awaiting transactions for Non Escrow Card Wallet Transaction" + " | "}{DateTime.Now}", false);

                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();

                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.TransactionJourney == TransactionJourneyStatusCodes.FioranoFirstFundingCompleted
                         && x.PaymentChannel == PaymentChannel.Card
                        //|| x.TransactionJourney == TransactionJourneyStatusCodes.FirstWalletFundingWasSuccessul
                        ).Take(1).ToListAsync();

                    var getNonEscrowTransactions = pendingTransactions.Where(x => x.Category == MerchantPaymentLinkCategory.Basic
                    || x.Category == MerchantPaymentLinkCategory.OneOffBasicLink).ToList();
                    _nonescrowLogger.LogRequest($"{"Job Service: Non Escrow Card Wallet Transaction. Total number of pending transactions" + " | " + pendingTransactions.Count + " | "}{DateTime.Now}", false);

                    if (getNonEscrowTransactions.Count == 0)
                        return "No record";
                    
                    await _transactions.ProcessTransactions(getNonEscrowTransactions);
                }

                Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";
            }
            catch (Exception ex)
            {
                _nonescrowLogger.LogRequest($"{"Job Service. Non Escrow Card Wallet Transaction" + "Error occured" + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", false);
                return "Error";
            }

        }
    }
}
