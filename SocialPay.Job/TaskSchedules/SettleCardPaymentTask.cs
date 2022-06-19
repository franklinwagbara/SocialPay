using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.PayWithCard;
using SocialPay.Job.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class SettleCardPaymentTask : CronJobService
    {


        private readonly IServiceProvider _scopeServiceProvider;

        public SettleCardPaymentTask(IServiceProvider serviceProvider, IScheduleConfig<SettleCardPaymentTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();

            var request = scope.ServiceProvider.GetRequiredService<ISettleCardPayment>();
            request.ProcessSettlement();

            return Task.CompletedTask;
        }
    }
}
