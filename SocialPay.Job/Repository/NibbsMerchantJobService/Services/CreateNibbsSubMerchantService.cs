using Microsoft.Extensions.DependencyInjection;
using SocialPay.Core.Services.QrCode;
using SocialPay.Domain;
using SocialPay.Job.Repository.NibbsMerchantJobService.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialPay.Helper.Dto.Request;
using SocialPay.Core.Services.Merchant;
using SocialPay.Helper.ViewModel;
using SocialPay.Job.Repository.NibbsMerchantJobService.Repository;
using SocialPay.Helper;

namespace SocialPay.Job.Repository.NibbsMerchantJobService.Services
{
    public class CreateNibbsSubMerchantService : ICreateNibbsSubMerchantService
    {
        private readonly NibbsQrJobCreateSubMerchantRepository _nibbsQrJobCreateSubMerchantRepository;
        public CreateNibbsSubMerchantService(IServiceProvider services,
                     NibbsQrJobCreateSubMerchantRepository nibbsQrJobCreateSubMerchantRepository)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));

            _nibbsQrJobCreateSubMerchantRepository = nibbsQrJobCreateSubMerchantRepository ?? throw new ArgumentNullException(nameof(nibbsQrJobCreateSubMerchantRepository));
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

                    var merchants = await context.MerchantQRCodeOnboarding.Where(x => x.Status == NibbsMerchantOnboarding.CreateAccount).ToListAsync();

                    if(merchants.Count > 0)
                        await _nibbsQrJobCreateSubMerchantRepository.ProcessTransactions(merchants);

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
