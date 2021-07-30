using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Merchant
{
    public class MerchantPersonalInfoRepository
    {
        private readonly SocialPayDbContext _context;
        public MerchantPersonalInfoRepository(SocialPayDbContext context, IServiceProvider services)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceProvider Services { get; }
        public async Task<BusinessInfoViewModel> GetCompleteMerchantPersonalDetailsAsync(long clientId)
        {
            try
            {

                var merchant = await _context.ClientAuthentication
                    .Include(x=>x.MerchantBusinessInfo)
                    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

                var result = new BusinessInfoViewModel
                {
                    BusinessPhoneNumber = merchant.PhoneNumber,
                    BusinessEmail = merchant.Email,
                    Tin = merchant.MerchantBusinessInfo.Select(x=>x.Tin).FirstOrDefault(),
                    BusinessName = merchant.MerchantBusinessInfo.Select(x => x.BusinessName).FirstOrDefault(),
                    ResponseCode = AppResponseCodes.Success
                };

                return result;
            }
            catch (Exception ex)
            {

                return new BusinessInfoViewModel { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task <List<ClientAuthenticationViewModel>> GetCompleteMerchantClientInfoDetailsAsync()
        {
            var result = new List<ClientAuthenticationViewModel>();

            try
            {
                using var scope = Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();


                var query = await _context.ClientAuthentication.Where(x => !_context.MerchantQRCodeOnboarding
                .Select(b => b.ClientAuthenticationId).Contains(x.ClientAuthenticationId)).ToListAsync();

                foreach (var item in query)
                {
                    result.Add(new ClientAuthenticationViewModel { PhoneNumber = item.PhoneNumber,
                    Email = item.Email});
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

    }
}
