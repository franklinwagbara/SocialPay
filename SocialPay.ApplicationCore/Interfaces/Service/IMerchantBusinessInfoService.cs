using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IMerchantBusinessInfoService
    {
        Task<List<BusinessInfoViewModel>> GetAllAsync();
        Task<BusinessInfoViewModel> GetMerchantBusinessInfo(long clientId);
        Task<BusinessInfoViewModel> GetMerchantBusinessEmailInfo(string email);
        Task<BusinessInfoViewModel> GetMerchantBusinessTinInfo(string email);
        Task<BusinessInfoViewModel> GetMerchantBusinessPhoneNumberInfo(string phoneNumber);
        Task<BusinessInfoViewModel> GetMerchantBusinessNameInfo(string businessName);
        Task<int> CountTotalMerchantsAsync();
        Task<bool> ExistsAsync(long clientId);
       // Task<bool> ExistsAsync(string refCode);
        Task UpdateAsync(BusinessInfoViewModel model);
    }
}
