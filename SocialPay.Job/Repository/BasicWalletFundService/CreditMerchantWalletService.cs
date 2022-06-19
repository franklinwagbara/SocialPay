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
    public class CreditMerchantWalletService : ICreditMerchantWalletService
    {
        private readonly CreditMerchantWalletTransactions _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(CreditMerchantWalletService));
        private readonly WalletJobLogger _walletLogger;

        public CreditMerchantWalletService(IServiceProvider services, CreditMerchantWalletTransactions transactions, WalletJobLogger walletLogger)
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
                _walletLogger.LogRequest($"{"Job Service" + "-" + "CreditMerchantWalletService" + " | " }{DateTime.Now}", false);
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();

                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.OrderStatus == 
                        TransactionJourneyStatusCodes.Pending 
                        && x.PaymentChannel != PaymentChannel.PayWithSpecta).Take(1).ToListAsync();
                    _walletLogger.LogRequest($"{"Job Service" + "-" + "CreditMerchantWalletService pending transactions" + " | " + pendingTransactions.Count + " | " }{DateTime.Now}", false);                    
                    
                    if (pendingTransactions.Count == 0)
                        return "No record";
                   
                    await _transactions.ProcessTransactions(pendingTransactions);
                }

               // Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";
            }
            catch (Exception ex)
            {
                _walletLogger.LogRequest($"{"Job Service" + "-" + "Error occured" + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", false);
                return "Error";
            }

        }

    }
}
