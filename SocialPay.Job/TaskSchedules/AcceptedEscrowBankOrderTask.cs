using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.AcceptedEscrowOrdersBankTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class AcceptedEscrowBankOrderTask : ScheduledProcessor
    {
        public AcceptedEscrowBankOrderTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 2 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            IAcceptedEscrowRequestBankTransaction reportGenerator = scopeServiceProvider.GetRequiredService<IAcceptedEscrowRequestBankTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
