using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.AcceptedEscrowOrdersBankTransaction;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    //public class AcceptedEscrowBankOrderTask : ScheduledProcessor
    //{
    //    public AcceptedEscrowBankOrderTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    //    {
    //    }

    //    protected override string Schedule => "*/" + 100 + " * * * *"; // every 4 min 

    //    public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
    //    {
    //        IAcceptedEscrowRequestBankTransaction reportGenerator = scopeServiceProvider.GetRequiredService<IAcceptedEscrowRequestBankTransaction>();
    //        reportGenerator.GetPendingTransactions();
    //        return Task.CompletedTask;
    //    }
    //}

    public class AcceptedEscrowBankOrderTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public AcceptedEscrowBankOrderTask(IServiceProvider serviceProvider, IScheduleConfig<AcceptedEscrowBankOrderTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            IAcceptedEscrowRequestBankTransaction reportGenerator = scope.ServiceProvider.GetRequiredService<IAcceptedEscrowRequestBankTransaction>();
            reportGenerator.GetPendingTransactions();
            return Task.CompletedTask;
        }
    }
}
