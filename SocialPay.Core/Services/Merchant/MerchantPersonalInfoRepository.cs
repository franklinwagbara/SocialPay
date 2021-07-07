using Microsoft.EntityFrameworkCore;
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
        public MerchantPersonalInfoRepository(SocialPayDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

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
    }
}
