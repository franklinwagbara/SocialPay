using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NonEscrowBankTransactions;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class NonEscrowBankTransactionTask : ScheduledProcessor
    {
        public NonEscrowBankTransactionTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 3 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            INonEscrowBankTransaction reportGenerator = scopeServiceProvider.GetRequiredService<INonEscrowBankTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
