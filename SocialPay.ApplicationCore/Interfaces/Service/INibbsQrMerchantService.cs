﻿using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface INibbsQrMerchantService
    {
        Task<List<NibbsQrMerchantViewModel>> GetAllAsync();
       // Task<List<PaymentLinkViewModel>> GetFundWalletByTenantAsync(long tenantId);
        Task<NibbsQrMerchantViewModel> GetMerchantInfo(long clientId);
        Task<NibbsQrMerchantViewModel> GetMerchantStatusInfo(long clientId, string status);
        //Task<NibbsQrMerchantViewModel> GetMerchantPersonalEmailInfo(string email);
        Task<int> CountTotalMerchantsAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<bool> ExistsAsync(long clientId, string status);
        Task AddAsync(NibbsQrMerchantViewModel model);
        Task UpdateAsync(NibbsQrMerchantViewModel model);
        Task DeleteAsync(int id);
    }
}
