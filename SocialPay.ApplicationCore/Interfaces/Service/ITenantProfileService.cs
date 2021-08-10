using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface ITenantProfileService
    {
        Task<List<TenantProfileViewModel>> GetAllAsync();
        Task<TenantProfileViewModel> GetProfileEmail(string email);
        Task<int> CountTotalProfilesAsync();
        Task<bool> ExistsAsync(long tenantId);
        Task<TenantProfileViewModel> AddAsync(TenantProfileViewModel model);
        Task UpdateAsync(TenantProfileViewModel model);
        Task DeleteAsync(int id);
    }
}
