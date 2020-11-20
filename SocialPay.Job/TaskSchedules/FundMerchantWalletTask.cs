using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class FundMerchantWalletTask : ScheduledProcessor
    {
        public FundMerchantWalletTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 5 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {

            IWalletTransactions reportGenerator = scopeServiceProvider.GetRequiredService<IWalletTransactions>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
