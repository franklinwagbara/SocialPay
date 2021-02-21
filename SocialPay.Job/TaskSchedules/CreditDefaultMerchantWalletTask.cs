using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.BasicWalletFundService;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class CreditDefaultMerchantWalletTask : ScheduledProcessor
    //{
    //    public CreditDefaultMerchantWalletTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 20 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {
    //        ICreditMerchantWalletService reportGenerator = scopeServiceProvider.GetRequiredService<ICreditMerchantWalletService>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}


    public class CreditDefaultMerchantWalletTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public CreditDefaultMerchantWalletTask(IServiceProvider serviceProvider, IScheduleConfig<CreditDefaultMerchantWalletTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            ICreditMerchantWalletService reportGenerator = scope.ServiceProvider.GetRequiredService<ICreditMerchantWalletService>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }

}
