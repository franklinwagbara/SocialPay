using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.DeliveryDayMerchantWalletTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class DeliveryDayWalletTask : ScheduledProcessor
    //{
    //    public DeliveryDayWalletTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 140 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {
    //        IDeliveryDayMerchantTransfer reportGenerator = scopeServiceProvider.GetRequiredService<IDeliveryDayMerchantTransfer>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}

    public class DeliveryDayWalletTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public DeliveryDayWalletTask(IServiceProvider serviceProvider, IScheduleConfig<DeliveryDayWalletTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            IDeliveryDayMerchantTransfer reportGenerator = scope.ServiceProvider.GetRequiredService<IDeliveryDayMerchantTransfer>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }

}
