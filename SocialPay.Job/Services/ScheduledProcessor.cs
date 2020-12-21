using Microsoft.Extensions.DependencyInjection;
using NCrontab;
using SocialPay.Core.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialPay.Job.Services
{
    public abstract class ScheduledProcessor : ScopedProcessor
    {
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly AppSettings _appSettings;

        protected abstract string Schedule { get; }

        public ScheduledProcessor(IServiceScopeFactory serviceScopeFactory
           ) : base(serviceScopeFactory)
        {
            _schedule = CrontabSchedule.Parse(Schedule);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                if (now > _nextRun)
                {
                    await Process();

                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }

                await Task.Delay(10000, stoppingToken); // 5 seconds delay

            } while (!stoppingToken.IsCancellationRequested);
        }


    }

}
