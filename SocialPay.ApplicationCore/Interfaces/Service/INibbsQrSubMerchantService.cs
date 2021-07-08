using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface INibbsQrSubMerchantService
    {
        Task<List<NibbsSubMerchantViewModel>> GetAllAsync();
       // Task<List<PaymentLinkViewModel>> GetFundWalletByTenantAsync(long tenantId);
        Task<NibbsSubMerchantViewModel> GetMerchantInfo(long Id);
        //Task<NibbsQrMerchantViewModel> GetMerchantPersonalEmailInfo(string email);
        Task<int> CountTotalMerchantsAsync();
        Task<bool> ExistsAsync(long clientId);
        Task AddAsync(NibbsSubMerchantViewModel model);
        Task UpdateAsync(NibbsSubMerchantViewModel model);
        Task DeleteAsync(int id);
    }
}
