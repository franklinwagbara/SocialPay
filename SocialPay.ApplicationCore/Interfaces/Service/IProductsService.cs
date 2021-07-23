using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IProductsService
    {
        Task<List<ProductsViewModel>> GetAllAsync();
        Task<List<ProductsViewModel>> GetProductsByCategory(long clientId);
        Task<List<ProductsViewModel>> GetProductByStoreId(long storeId);
        Task<ProductsViewModel> GetProductById(long productId);
       // Task<ProductsViewModel> GetProductByProductNameAndStoreId(long productId);
        Task<ProductsViewModel> GetProductByNameCatIdAndClientId(string productName, long categoryId, long storeId);
        Task<int> CountTotalProductsAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<ProductsViewModel> AddAsync(ProductsViewModel model);
        Task UpdateAsync(ProductsViewModel model);
        Task DeleteAsync(int id);
    }
}
