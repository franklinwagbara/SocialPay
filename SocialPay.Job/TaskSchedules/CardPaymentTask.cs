using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.PayWithCard;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class CardPaymentTask : ScheduledProcessor
    //{
    //    public CardPaymentTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    //protected override string Schedule => "*/" + 30 + " * * * *"; // every 4 min 
    //    protected override string Schedule => "20 16 * * * "; // every 4 min 
    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {
    //        IPayWithCardTransaction reportGenerator = scopeServiceProvider.GetRequiredService<IPayWithCardTransaction>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}

    public class CardPaymentTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public CardPaymentTask(IServiceProvider serviceProvider, IScheduleConfig<CardPaymentTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            IPayWithCardTransaction reportGenerator = scope.ServiceProvider.GetRequiredService<IPayWithCardTransaction>();
            reportGenerator.GetPendingTransactions();

            return Task.CompletedTask;
        }
    }

}
