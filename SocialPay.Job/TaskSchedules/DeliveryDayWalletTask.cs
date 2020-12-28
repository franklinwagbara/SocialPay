using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.DeliveryDayMerchantWalletTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class DeliveryDayWalletTask : ScheduledProcessor
    {
        public DeliveryDayWalletTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 2 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            IDeliveryDayMerchantTransfer reportGenerator = scopeServiceProvider.GetRequiredService<IDeliveryDayMerchantTransfer>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
