using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.SerilogService.WalletJob;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeliveryDayMerchantWalletTransaction
{
    public class DeliveryDayMerchantTransfer : IDeliveryDayMerchantTransfer
    {
        private readonly DeliveryDayTransferService _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(DeliveryDayMerchantTransfer));
        private readonly WalletJobLogger _walletLogger;
        public DeliveryDayMerchantTransfer(IServiceProvider services, DeliveryDayTransferService transactions, WalletJobLogger walletLogger)
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
                _walletLogger.LogRequest($"{"Job Service" + "-" + "Tasks starts to process DeliveryDayMerchantTransfer transactions" + " | " }{DateTime.Now}", false);

                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    DateTime nextDay = DateTime.Now.Date.AddDays(1);

                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.DeliveryDayTransferStatus == TransactionJourneyStatusCodes.Pending
                        && x.DeliveryFinalDate.Day == DateTime.Now.Day).ToListAsync();

                    var getvalidRequest = pendingTransactions.Where(x => x.Category
                       == MerchantPaymentLinkCategory.Escrow
                   || x.Category == MerchantPaymentLinkCategory.OneOffEscrowLink).ToList();

                    _walletLogger.LogRequest($"{"Total number of pending transactions" + " | " + pendingTransactions.Count + " | "}{DateTime.Now}", false);

                    if (getvalidRequest.Count == 0)
                        return "No record";

                    await _transactions.ProcessTransactions(getvalidRequest);
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
