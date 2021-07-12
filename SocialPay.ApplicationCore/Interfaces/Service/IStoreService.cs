using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IStoreService
    {
        Task<List<StoreViewModel>> GetAllAsync();
        Task<List<StoreViewModel>> GetStoresByClientId(long clientId);
        Task<StoreViewModel> GetStoreById(long storeId);
        Task<StoreViewModel> GetStoreById(long storeId, long clientId);
        Task<StoreViewModel> GetStoreName(string storeName);
        Task<int> CountTotalStoressAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<StoreViewModel> AddAsync(StoreViewModel model);
        Task UpdateAsync(StoreViewModel model);
        Task DeleteAsync(int id);
    }
}
