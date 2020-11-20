using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.AcceptedEscrowOrdersWalletTransaction;
using SocialPay.Job.Repository.DeclinedEscrowWalletTransaction;
using SocialPay.Job.Repository.PayWithCard;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class DeclinedEscrowWalletTask : ScheduledProcessor
    {
        public DeclinedEscrowWalletTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 3 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            IDeclineEscrowWalletTransaction reportGenerator = scopeServiceProvider.GetRequiredService<IDeclineEscrowWalletTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
