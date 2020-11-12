using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace SocialPay.Job.Services
{
    public abstract class ScopedProcessor : BackgroundService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        public ScopedProcessor(IServiceScopeFactory serviceScopeFactory) : base()
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task Process()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                await ProcessInScope(scope.ServiceProvider);
            }
        }

        public abstract Task ProcessInScope(IServiceProvider scopeServiceProvider);
    }

}
