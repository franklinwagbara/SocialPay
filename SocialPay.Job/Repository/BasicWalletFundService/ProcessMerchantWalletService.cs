using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.SerilogService.WalletJob;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.BasicWalletFundService
{
    public class ProcessMerchantWalletService : IProcessMerchantWalletService
    {
        private readonly ProcessMerchantWalletTransactions _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ProcessMerchantWalletService));
        private readonly WalletJobLogger _walletLogger;
        public ProcessMerchantWalletService(IServiceProvider services, ProcessMerchantWalletTransactions transactions, WalletJobLogger walletLogger)
        {
            Services = services;
            _transactions = transactions;
            _walletLogger = walletLogger;
        }
        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                _walletLogger.LogRequest($"{"Job Service" + "-" + "to fetch awaiting transactions for Non Escrow Card Wallet Transaction" + " | "}{DateTime.Now}", false);

                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();

                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.TransactionJourney == TransactionJourneyStatusCodes.ProcessingFinalWalletRequest
                         && x.PaymentChannel == PaymentChannel.Card
                        ).Take(1).ToListAsync();

                    var getNonEscrowTransactions = pendingTransactions.Where(x => x.Category == MerchantPaymentLinkCategory.Basic
                    || x.Category == MerchantPaymentLinkCategory.OneOffBasicLink).ToList();
                    _walletLogger.LogRequest($"{"Job Service: Non Escrow Card Wallet Transaction. Total number of pending transactions" + " | " + pendingTransactions.Count + " | "}{DateTime.Now}", false);

                    if (getNonEscrowTransactions.Count == 0)
                        return "No record";

                    await _transactions.ProcessTransactions(getNonEscrowTransactions);
                }

                Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";
            }
            catch (Exception ex)
            {
                _walletLogger.LogRequest($"{"Job Service. Non Escrow Card Wallet Transaction" + "Error occured" + " | " + ex.Message.ToString() + " | " }{DateTime.Now}", true);
                return "Error";
            }

        }

    }
}
