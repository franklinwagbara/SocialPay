using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.DeliveryDayBankTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class DeliveryDayBankTask : ScheduledProcessor
    //{
    //    public DeliveryDayBankTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 135 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {
    //        IDeliveryDayBankTransaction reportGenerator = scopeServiceProvider.GetRequiredService<IDeliveryDayBankTransaction>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}

    public class DeliveryDayBankTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public DeliveryDayBankTask(IServiceProvider serviceProvider, IScheduleConfig<DeliveryDayBankTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            IDeliveryDayBankTransaction reportGenerator = scope.ServiceProvider.GetRequiredService<IDeliveryDayBankTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }

}
