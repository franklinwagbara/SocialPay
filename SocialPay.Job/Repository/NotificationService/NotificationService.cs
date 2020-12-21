using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NotificationService
{
    public class NotificationService : INotificationServices
    {
        private readonly NotificationTransactions _transactions;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NotificationService));
        public NotificationService(NotificationTransactions transactions, IServiceProvider services)
        {
            Services = services;
            _transactions = transactions;
        }
        public IServiceProvider Services { get; }

        public async Task<string> GetPendingTransactions()
        {
            try
            {
                  _log4net.Info("Job Service" + "-"+ "Tasks starts to get all notification transactions" + " | " + DateTime.Now);
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    DateTime nextDay = DateTime.Now.Date.AddDays(1);
                    var pendingTransactions = await context.TransactionLog
                        .Where(x =>x.IsNotified == false && x.ActivityStatus == TransactionJourneyStatusCodes.Pending
                        && x.DeliveryDate.Day == nextDay.Day).ToListAsync();

                    var getvalidRequest = pendingTransactions.Where(x => x.Category 
                         == MerchantPaymentLinkCategory.Escrow
                     || x.Category == MerchantPaymentLinkCategory.OneOffEscrowLink).ToList();
                     _log4net.Info("Job Service" + "-" + "Total number of pending transactions" + " | " + pendingTransactions.Count + " | " + DateTime.Now);
                    if (getvalidRequest.Count == 0)
                        return "No record";
                    await _transactions.InitiatePendingNotifications(getvalidRequest);
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
