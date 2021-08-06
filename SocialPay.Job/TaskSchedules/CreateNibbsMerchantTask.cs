
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NibbsMerchantJobService.Interface;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class CreateNibbsMerchantTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public CreateNibbsMerchantTask(IServiceProvider serviceProvider, IScheduleConfig<CreateNibbsMerchantTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            var request = scope.ServiceProvider.GetRequiredService<ICreateNibbsMerchantService>();
            request.GetPendingTransactions();

            return Task.CompletedTask;
        }
    }

}
