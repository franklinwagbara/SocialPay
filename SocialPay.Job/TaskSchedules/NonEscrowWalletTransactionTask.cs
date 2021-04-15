using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NonEscrowCardWalletTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class NonEscrowWalletTransactionTask : ScheduledProcessor
    //{
    //    public NonEscrowWalletTransactionTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 30 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {
    //        INonEscrowCardWalletTransaction reportGenerator = scopeServiceProvider.GetRequiredService<INonEscrowCardWalletTransaction>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}

    public class NonEscrowWalletTransactionTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public NonEscrowWalletTransactionTask(IServiceProvider serviceProvider, IScheduleConfig<NonEscrowWalletTransactionTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            INonEscrowCardWalletTransaction reportGenerator = scope.ServiceProvider.GetRequiredService<INonEscrowCardWalletTransaction>();
            reportGenerator.GetPendingTransactions();

            return Task.CompletedTask;
        }
    }

}
