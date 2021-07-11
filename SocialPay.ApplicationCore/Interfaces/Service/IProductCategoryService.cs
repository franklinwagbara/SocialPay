using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryViewModel>> GetAllAsync();
        Task<List<ProductCategoryViewModel>> GetAllByClientId(long clientId);
        Task<ProductCategoryViewModel> GetCategoryById(long categoryId);
        Task<ProductCategoryViewModel> GetCategoryByIdAndCatId(long categoryId, long clientId);
        Task<ProductCategoryViewModel> GetCategoryByNameAndClientId(string catName, long clientId);
        Task<bool> ExistsAsync(long categoryId);
        Task<ProductCategoryViewModel> AddAsync(ProductCategoryViewModel model);
        Task UpdateAsync(ProductCategoryViewModel model);
        Task DeleteAsync(int id);
    }
}
