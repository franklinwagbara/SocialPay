using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.AcceptedOrders;
using SocialPay.Job.Repository.PayWithCard;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class AcceptedOrderTask : ScheduledProcessor
    {
        public AcceptedOrderTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 3 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            IAcceptedOrders reportGenerator = scopeServiceProvider.GetRequiredService<IAcceptedOrders>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
