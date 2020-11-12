using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedOrders
{
    public class AcceptedOrders : IAcceptedOrders
    {
        private readonly AcceptedOrderTransactions _transactions;
        public AcceptedOrders(AcceptedOrderTransactions transactions, IServiceProvider services)
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
                        .Where(x => x.OrderStatus == OrderStatusCode.Approved
                        && x.Category == MerchantPaymentLinkCategory.Escrow
                        || x.Category == MerchantPaymentLinkCategory.OneOffEscrowLink
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
