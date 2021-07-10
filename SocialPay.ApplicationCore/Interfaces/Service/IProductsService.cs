using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IProductsService
    {
        Task<List<ProductsViewModel>> GetAllAsync();
        Task<List<ProductsViewModel>> GetProductsByClientId(long clientId);
        Task<ProductsViewModel> GetProductById(long productId);
        Task<int> CountTotalProductsAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<ProductsViewModel> AddAsync(ProductsViewModel model);
        Task UpdateAsync(ProductsViewModel model);
        Task DeleteAsync(int id);
    }
}
