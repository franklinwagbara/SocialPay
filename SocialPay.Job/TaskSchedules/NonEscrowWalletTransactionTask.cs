using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NonEscrowCardWalletTransaction;
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

        protected override string Schedule => "*/" + 3 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            INonEscrowCardWalletTransaction reportGenerator = scopeServiceProvider.GetRequiredService<INonEscrowCardWalletTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
