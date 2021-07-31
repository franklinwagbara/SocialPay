using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IMerchantBankingInfoService
    {
        Task<List<MerchantBankInfoViewModel>> GetAllAsync();
        Task<MerchantBankInfoViewModel> GetMerchantBankInfo(long clientId);
        Task<MerchantBankInfoViewModel> GetMerchantByNuban(string phoneNumber);
        Task<int> CountTotalMerchantsAsync();
        Task<bool> ExistsAsync(long clientId);
        Task UpdateAsync(MerchantBankInfoViewModel model);
    }
}
