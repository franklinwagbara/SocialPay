
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NibbsMerchantJobService.Interface;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class BindMerchantTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public BindMerchantTask(IServiceProvider serviceProvider, IScheduleConfig<BindMerchantTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            var request = scope.ServiceProvider.GetRequiredService<IBindMerchantService>();
            request.GetPendingTransactions();

            return Task.CompletedTask;
        }
    }

}
