using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Job.Repository.NibbsMerchantJobService.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialPay.Helper.ViewModel;
using SocialPay.Job.Repository.NibbsMerchantJobService.Repository;
using SocialPay.Helper;

namespace SocialPay.Job.Repository.NibbsMerchantJobService.Services
{
    public class BindMerchantService : IBindMerchantService
    {
        private readonly BindMerchantServiceRepository _bindMerchantServiceRepository;
        public BindMerchantService(IServiceProvider services,
                     BindMerchantServiceRepository bindMerchantServiceRepository)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));

            _bindMerchantServiceRepository = bindMerchantServiceRepository ?? throw new ArgumentNullException(nameof(bindMerchantServiceRepository));
        }
        public IServiceProvider Services { get; }
        public async Task<string> GetPendingTransactions()
        {
            var result = new List<ClientAuthenticationViewModel>();


            try
            {

                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();

                    var merchants = await context.ClientAuthentication.Where(x => x.ClientAuthenticationId == 10380).ToListAsync();
                   //var merchants = await context.ClientAuthentication.Where(x => x.QrCodeStatus == NibbsMerchantOnboarding.SubAccount).ToListAsync();

                    if(merchants.Count > 0)
                        await _bindMerchantServiceRepository.ProcessTransactions(merchants);

                    return "Task Completed";
                }

            }
            catch (Exception ex)
            {
                return "Error";
            }

        }
    }
}
