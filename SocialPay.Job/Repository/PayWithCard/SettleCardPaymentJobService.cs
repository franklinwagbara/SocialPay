using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.SerilogService.PayWithCardJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.PayWithCard
{
    public class SettleCardPaymentJobService : ISettleCardPayment
    {
        
        private readonly PayWithCardJobLogger _paywithcardjobLogger;
        private readonly MerchantBankSettlementService _merchantBankSettlementService;
        public SettleCardPaymentJobService( PayWithCardJobLogger paywithcardjobLogger, IServiceProvider services, MerchantBankSettlementService merchantBankSettlementService
        )
        {
           
           _paywithcardjobLogger = paywithcardjobLogger;
            Services = services;
            _merchantBankSettlementService = merchantBankSettlementService;
        }
        public IServiceProvider Services { get; }




        public async Task<string> ProcessSettlement()
        {
            _paywithcardjobLogger.LogRequest($"{"Job Service " + "-" + "Process merchant settlement task" + " | "}{DateTime.Now}", false);
            try
            {


                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();

                    var pendingTransactions = await context.TransactionLog
                        .Where(x => x.TransactionJourney ==
                        TransactionJourneyStatusCodes.Approved && x.PaymentChannel == PaymentChannel.Card).Take(1).ToListAsync();

                    //var pendingTransactions = await context.TransactionLog
                    //    .Where(x => x.TransactionLogId == 20108).Take(5).ToListAsync();

                    var getNonEscrowTransactions = pendingTransactions.Where(x => x.LinkCategory == MerchantPaymentLinkCategory.Basic
                     || x.LinkCategory == MerchantPaymentLinkCategory.OneOffBasicLink).ToList();

                    _paywithcardjobLogger.LogRequest($"{"Job Service: Total number of pending transactions" + " | " + pendingTransactions.Count + " | "}{DateTime.Now}", false);

                    if (getNonEscrowTransactions.Count == 0)
                        return "No record";

                    await _merchantBankSettlementService.ProcessTransactions(getNonEscrowTransactions, context);
                }

                Console.WriteLine("GenerateDailyReport : " + DateTime.Now.ToString());

                return "GenerateDailyReport";

            }
            catch(Exception ex)
            {
                _paywithcardjobLogger.LogRequest($"{"Job Service: An error occured while fetching transactions" + " | " + ex.Message.ToString() + " | " }{DateTime.Now}", true);

                return "Error";

            }

        }
    }
}
