using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NonEscrowOtherWalletTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class NonEscrowOtherWalletTransactionTask : ScheduledProcessor
    //{
    //    public NonEscrowOtherWalletTransactionTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 35 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {
    //        INonEscrowOtherWalletTransaction reportGenerator = scopeServiceProvider.GetRequiredService<INonEscrowOtherWalletTransaction>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}

    public class NonEscrowOtherWalletTransactionTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public NonEscrowOtherWalletTransactionTask(IServiceProvider serviceProvider, IScheduleConfig<NonEscrowOtherWalletTransactionTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            INonEscrowOtherWalletTransaction reportGenerator = scope.ServiceProvider.GetRequiredService<INonEscrowOtherWalletTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }

}
