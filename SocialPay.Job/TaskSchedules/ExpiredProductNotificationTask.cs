using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NotificationService;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class ExpiredProductNotificationTask : ScheduledProcessor
    {
        public ExpiredProductNotificationTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 50 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {

            INotificationServices reportGenerator = scopeServiceProvider.GetRequiredService<INotificationServices>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
