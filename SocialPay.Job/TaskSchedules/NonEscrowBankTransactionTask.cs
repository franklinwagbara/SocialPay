using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NonEscrowBankTransactions;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class NonEscrowBankTransactionTask : ScheduledProcessor
    //{
    //    public NonEscrowBankTransactionTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 40 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {
    //        INonEscrowBankTransaction reportGenerator = scopeServiceProvider.GetRequiredService<INonEscrowBankTransaction>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}




    public class NonEscrowBankTransactionTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public NonEscrowBankTransactionTask(IServiceProvider serviceProvider, IScheduleConfig<NonEscrowBankTransactionTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            INonEscrowBankTransaction reportGenerator = scope.ServiceProvider.GetRequiredService<INonEscrowBankTransaction>();
            reportGenerator.GetPendingTransactions();
           
            return Task.CompletedTask;
        }
    }

}
