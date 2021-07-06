using SocialPay.Helper.ViewModel;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IPersonalInfoService
    {
       // Task<List<PaymentLinkViewModel>> GetAllAsync();
       // Task<List<PaymentLinkViewModel>> GetFundWalletByTenantAsync(long tenantId);
        Task<PersonalInfoViewModel> GetMerchantPersonalInfo(long clientId);
        Task<PersonalInfoViewModel> GetMerchantPersonalEmailInfo(string email);
        Task<PersonalInfoViewModel> GetMerchantPersonalPhoneNumberInfo(string phoneNumber);
        Task<PersonalInfoViewModel> GetMerchantPersonalBvnInfo(string phoneNumber);
        Task<int> CountTotalFundAsync();
        Task<bool> ExistsAsync(long clientId);
        //Task<long> AddAsync(PaymentLinkViewModel model);
        Task UpdateAsync(PersonalInfoViewModel model);
        //Task DeleteAsync(int id);
    }
}
