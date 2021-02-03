using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.AcceptedEscrowOrdersWalletTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class AcceptedWalletOrderTask : ScheduledProcessor
    //{
    //    public AcceptedWalletOrderTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 60 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {
    //        IAcceptedEscrowOrders reportGenerator = scopeServiceProvider.GetRequiredService<IAcceptedEscrowOrders>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}


    public class AcceptedWalletOrderTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public AcceptedWalletOrderTask(IServiceProvider serviceProvider, IScheduleConfig<AcceptedWalletOrderTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            IAcceptedEscrowOrders reportGenerator = scope.ServiceProvider.GetRequiredService<IAcceptedEscrowOrders>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }

}
