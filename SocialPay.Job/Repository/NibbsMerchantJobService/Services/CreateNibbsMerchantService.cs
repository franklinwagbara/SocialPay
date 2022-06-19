using Microsoft.Extensions.DependencyInjection;
using SocialPay.Core.Services.QrCode;
using SocialPay.Domain;
using SocialPay.Job.Repository.NibbsMerchantJobService.Interface;
using System;
using System.Collections.Generic;
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
        private readonly NibbsQrJobCreateMerchantRepository _nibbsQrJobCreateMerchantRepository;
        public CreateNibbsMerchantService(IServiceProvider services,
                     NibbsQrJobCreateMerchantRepository nibbsQrJobCreateMerchantRepository)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));

            //_nibbsQrBaseService = nibbsQrBaseService ?? throw new ArgumentNullException(nameof(nibbsQrBaseService));
            //_merchantPersonalInfoRepository = merchantPersonalInfoRepository ?? throw new ArgumentNullException(nameof(merchantPersonalInfoRepository));
            //_nibbsQrRepository = nibbsQrRepository ?? throw new ArgumentNullException(nameof(nibbsQrRepository));
            _nibbsQrJobCreateMerchantRepository = nibbsQrJobCreateMerchantRepository ?? throw new ArgumentNullException(nameof(nibbsQrJobCreateMerchantRepository));
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


                    //var query = await context.ClientAuthentication
                    //.Where(x => x.LastDateModified < DateTime.Now.AddMinutes(-10))
                    //.Where(x => x.StatusCode == "00" && !context.MerchantQRCodeOnboarding
                    //.Select(b => b.ClientAuthenticationId).Contains(x.ClientAuthenticationId)).Take(5).ToListAsync();

                    var query = await context.ClientAuthentication.Where(x => x.ClientAuthenticationId == 10380).ToListAsync();

                    var model = new DefaultMerchantRequestDto();

                    var qrRequest = new List<NibbsQrMerchantViewModel>();

                    foreach (var item in query)
                    {
                        qrRequest.Add(new NibbsQrMerchantViewModel
                        {
                            IsDeleted = false,
                            Address = item.FullName,
                            Contact = item.PhoneNumber,
                            Email = item.Email,
                            Fee = 00,
                            Name = item.FullName,
                            Phone = item.PhoneNumber,
                            ClientAuthenticationId = item.ClientAuthenticationId
                        });
                    }

                    await _nibbsQrJobCreateMerchantRepository.ProcessTransactions(qrRequest);

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
