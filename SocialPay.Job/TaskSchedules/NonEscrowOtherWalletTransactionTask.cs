using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NonEscrowOtherWalletTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class NonEscrowOtherWalletTransactionTask : ScheduledProcessor
    {
        public NonEscrowOtherWalletTransactionTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 25 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            INonEscrowOtherWalletTransaction reportGenerator = scopeServiceProvider.GetRequiredService<INonEscrowOtherWalletTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
