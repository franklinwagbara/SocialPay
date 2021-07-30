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

namespace SocialPay.Job.Repository.NibbsMerchantJobService.Services
{
    public class CreateNibbsMerchantService : ICreateNibbsMerchantService
    {
        private readonly NibbsQrBaseService _nibbsQrBaseService;
        private readonly NibbsQrRepository _nibbsQrRepository;
        private readonly MerchantPersonalInfoRepository _merchantPersonalInfoRepository;
        private readonly NibbsQrJobRepository _nibbsQrJobRepository;
        public CreateNibbsMerchantService(IServiceProvider services,
                     NibbsQrJobRepository nibbsQrJobRepository)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));

            //_nibbsQrBaseService = nibbsQrBaseService ?? throw new ArgumentNullException(nameof(nibbsQrBaseService));
            //_merchantPersonalInfoRepository = merchantPersonalInfoRepository ?? throw new ArgumentNullException(nameof(merchantPersonalInfoRepository));
            //_nibbsQrRepository = nibbsQrRepository ?? throw new ArgumentNullException(nameof(nibbsQrRepository));
            _nibbsQrJobRepository = nibbsQrJobRepository ?? throw new ArgumentNullException(nameof(nibbsQrJobRepository));
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

                    var query = await context.ClientAuthentication.Where(x => !context.MerchantQRCodeOnboarding
                      .Select(b => b.ClientAuthenticationId).Contains(x.ClientAuthenticationId)).ToListAsync();

                    var model = new DefaultMerchantRequestDto();

                    var qrRequest = new List<NibbsQrMerchantViewModel>();

                    foreach (var item in query)
                    {
                      
                        qrRequest.Add(new NibbsQrMerchantViewModel
                        {
                            ClientAuthenticationId = item.ClientAuthenticationId,
                            IsDeleted = false,
                            Address = "",
                            Contact = item.PhoneNumber,
                            Email = item.Email,
                            Fee = 00,
                            Name = item.FullName,
                            Phone = item.PhoneNumber
                        });
                    }

                    await _nibbsQrJobRepository.ProcessTransactions(qrRequest);                  

                    return "Task Completed";
                }


                // var query = await _merchantPersonalInfoRepository.GetCompleteMerchantClientInfoDetailsAsync();

            }
            catch (Exception ex)
            {
                return "Error";
            }

        }
    }
}
