using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.IntraBankService;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class IntraBankTask : ScheduledProcessor
    {
        public IntraBankTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 3 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            IIntraBankTransferService reportGenerator = scopeServiceProvider.GetRequiredService<IIntraBankTransferService>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
