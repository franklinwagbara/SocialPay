using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IPaymentLinkService
    {
        Task<List<PaymentLinkViewModel>> GetAllAsync();
        Task<List<PaymentLinkViewModel>> GetFundWalletByTenantAsync(long tenantId);
        Task<PaymentLinkViewModel> GetFundWalletByReferenceAsync(string reference);
        Task<int> CountTotalFundAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<long> AddAsync(PaymentLinkViewModel model);
        Task UpdateAsync(PaymentLinkViewModel model);
        Task DeleteAsync(int id);
    }
}
