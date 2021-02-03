using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.DeclinedEscrowWalletTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class DeclinedEscrowWalletTask : ScheduledProcessor
    //{
    //    public DeclinedEscrowWalletTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 40 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {
    //        IDeclineEscrowWalletTransaction reportGenerator = scopeServiceProvider.GetRequiredService<IDeclineEscrowWalletTransaction>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}



    public class DeclinedEscrowWalletTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public DeclinedEscrowWalletTask(IServiceProvider serviceProvider, IScheduleConfig<DeclinedEscrowWalletTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            IDeclineEscrowWalletTransaction reportGenerator = scope.ServiceProvider.GetRequiredService<IDeclineEscrowWalletTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }

}
