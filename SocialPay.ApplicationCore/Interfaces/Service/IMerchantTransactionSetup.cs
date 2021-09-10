using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IMerchantTransactionSetup
    {
        Task<List<MerchantTransactionSetupViewModel>> GetAllAsync();
        Task<MerchantTransactionSetupViewModel> GetMerchantInfo(long clientId);
        Task<MerchantTransactionSetupViewModel> GetMerchantValidationInfo(long clientId, string pin);
        Task<MerchantTransactionSetupViewModel> AddAsync(MerchantTransactionSetupViewModel model);
        Task<int> CountTotalMerchantsAsync();
        Task<bool> ExistsAsync(long clientId);
        Task UpdateAsync(MerchantTransactionSetupViewModel model);
    }
}
