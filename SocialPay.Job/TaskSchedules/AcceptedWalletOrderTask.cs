using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.AcceptedEscrowOrdersWalletTransaction;
using SocialPay.Job.Repository.PayWithCard;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class AcceptedWalletOrderTask : ScheduledProcessor
    {
        public AcceptedWalletOrderTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 5 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            IAcceptedEscrowOrders reportGenerator = scopeServiceProvider.GetRequiredService<IAcceptedEscrowOrders>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
