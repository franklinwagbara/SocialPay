using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.DeliveryDayBankTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class DeliveryDayBankTask : ScheduledProcessor
    {
        public DeliveryDayBankTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 5 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            IDeliveryDayBankTransaction reportGenerator = scopeServiceProvider.GetRequiredService<IDeliveryDayBankTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
