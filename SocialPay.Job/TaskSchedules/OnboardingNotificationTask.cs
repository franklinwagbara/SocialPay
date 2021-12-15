using Microsoft.Extensions.DependencyInjection;
using SocialPay.Job.Repository.OnboardingNotification;
using SocialPay.Job.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.TaskSchedules
{
    public class OnboardingNotificationTask : CronJobService
    {
        private readonly IServiceProvider _scopeServiceProvider;

        public OnboardingNotificationTask(IServiceProvider serviceProvider, IScheduleConfig<OnboardingNotificationTask> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scopeServiceProvider = serviceProvider;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeServiceProvider.CreateScope();
            IOnboardingNotificationService reportGenerator = scope.ServiceProvider.GetRequiredService<IOnboardingNotificationService>();
            reportGenerator.SendNotificationToCompleteOnboarding();
            return Task.CompletedTask;
        }
    }

}
