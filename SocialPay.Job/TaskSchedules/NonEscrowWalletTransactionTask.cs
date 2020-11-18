using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NonEscrowWalletTransaction;
using SocialPay.Job.Repository.PayWithCard;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class NonEscrowWalletTransactionTask : ScheduledProcessor
    {
        public NonEscrowWalletTransactionTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 5 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            INonEscrowWalletTransaction reportGenerator = scopeServiceProvider.GetRequiredService<INonEscrowWalletTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
