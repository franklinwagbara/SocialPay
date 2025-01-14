﻿using SocialPay.Helper.ViewModel;
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
        Task<PersonalInfoViewModel> GetMerchantPersonalFullname(string fullName);
        Task<int> CountTotalFundAsync();
        Task<bool> ExistsAsync(long clientId);

        Task<bool> ExistsAsync(string refCode);
        //Task<long> AddAsync(PaymentLinkViewModel model);
        Task UpdateAsync(PersonalInfoViewModel model);
        //Task DeleteAsync(int id);
    }
}
