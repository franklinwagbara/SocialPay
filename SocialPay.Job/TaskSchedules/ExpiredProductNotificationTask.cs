using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NotificationService;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class ExpiredProductNotificationTask : ScheduledProcessor
    //{
    //    public ExpiredProductNotificationTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 180 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {

    //        INotificationServices reportGenerator = scopeServiceProvider.GetRequiredService<INotificationServices>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}

    public class ExpiredProductNotificationTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public ExpiredProductNotificationTask(IServiceProvider serviceProvider, IScheduleConfig<ExpiredProductNotificationTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            INotificationServices reportGenerator = scope.ServiceProvider.GetRequiredService<INotificationServices>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }

}
