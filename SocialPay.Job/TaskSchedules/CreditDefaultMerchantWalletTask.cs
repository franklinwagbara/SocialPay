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

        protected override string Schedule => "*/" + 20 + " * * * *"; // every 4 min 
        //protected override string Schedule => "50 0 10,15/12 * *"; // every 4 min 
       //////// protected override string Schedule => "20 16 * * * "; // every 4 min 
       // protected override string Schedule => "49 11-15 * * *"; // every 4 min 
       // protected override string Schedule => "55 0-5 11 * *"; // every 4 min 

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            ICreditMerchantWalletService reportGenerator = scopeServiceProvider.GetRequiredService<ICreditMerchantWalletService>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
