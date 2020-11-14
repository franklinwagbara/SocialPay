using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.BasicWalletFundService;
using SocialPay.Job.Services;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class CreditDefaultMerchantWalletTask : ScheduledProcessor
    {
        public CreditDefaultMerchantWalletTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/" + 3 + " * * * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            ICreditMerchantWalletService reportGenerator = scopeServiceProvider.GetRequiredService<ICreditMerchantWalletService>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
