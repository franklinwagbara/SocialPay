using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IMerchantBusinessInfoService
    {
       // Task<List<PaymentLinkViewModel>> GetAllAsync();
       // Task<List<PaymentLinkViewModel>> GetFundWalletByTenantAsync(long tenantId);
        Task<BusinessInfoViewModel> GetMerchantBusinessInfo(long clientId);
        //Task<int> CountTotalFundAsync();
        //Task<bool> ExistsAsync(long clientId);
        //Task<long> AddAsync(PaymentLinkViewModel model);
        //Task UpdateAsync(PaymentLinkViewModel model);
        //Task DeleteAsync(int id);
    }
}
