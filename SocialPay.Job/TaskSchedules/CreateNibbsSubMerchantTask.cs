
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.NibbsMerchantJobService.Interface;
using SocialPay.Job.Repository.PayWithCard;
using SocialPay.Job.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class CreateNibbsSubMerchantTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public CreateNibbsSubMerchantTask(IServiceProvider serviceProvider, IScheduleConfig<CreateNibbsSubMerchantTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            var request = scope.ServiceProvider.GetRequiredService<ICreateNibbsSubMerchantService>();
            request.GetPendingTransactions();

            return Task.CompletedTask;
        }
    }

}
