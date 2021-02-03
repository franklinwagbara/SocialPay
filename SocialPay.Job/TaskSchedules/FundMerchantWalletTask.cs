using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class FundMerchantWalletTask : ScheduledProcessor
    //{
    //    public FundMerchantWalletTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 5 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {

    //        IWalletTransactions reportGenerator = scopeServiceProvider.GetRequiredService<IWalletTransactions>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}

    public class FundMerchantWalletTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public FundMerchantWalletTask(IServiceProvider serviceProvider, IScheduleConfig<FundMerchantWalletTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            IWalletTransactions reportGenerator = scope.ServiceProvider.GetRequiredService<IWalletTransactions>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }

}
