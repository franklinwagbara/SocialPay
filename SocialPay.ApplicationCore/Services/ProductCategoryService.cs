using AutoMapper;
using SocialPay.ApplicationCore.Interfaces.Repositories;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Domain.Entities;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Services
{

    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<ProductCategory> _category;

        public ProductCategoryService(IAsyncRepository<ProductCategory> category)
        {
            _category = category;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<ProductCategory, ProductCategoryViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<ProductCategoryViewModel>> GetAllAsync()
        {
            var categories = await _category.GetAllAsync();

            return _mapper.Map<List<ProductCategory>, List<ProductCategoryViewModel>>(categories);
        }

        public async Task<List<ProductCategoryViewModel>> GetAllByClientId(long clientId)
        {
            var categories = await _category.GetAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<List<ProductCategory>, List<ProductCategoryViewModel>>(categories);
        }

        public async Task<ProductCategoryViewModel> GetCategoryByIdAndCatId(long categoryId, long clientId)
        {
            var categories = await _category
                .GetSingleAsync(x => x.ProductCategoryId == categoryId && x.ClientAuthenticationId == clientId);

            return _mapper.Map<ProductCategory, ProductCategoryViewModel>(categories);
        }
        //GetCategoryByIdAndCatId
        public async Task<ProductCategoryViewModel> GetCategoryById(long categoryId)
        {
            var category = await _category.GetSingleAsync(x => x.ProductCategoryId == categoryId);

            return _mapper.Map<ProductCategory, ProductCategoryViewModel>(category);
        }
        public async Task<ProductCategoryViewModel> GetCategoryByNameAndClientId(string catName, long clientId)
        {
            var category = await _category
                .GetSingleAsync(x => x.CategoryName == catName && x.ClientAuthenticationId == clientId);

            return _mapper.Map<ProductCategory, ProductCategoryViewModel>(category);
        }


        //GetCategoryByNameAndClientId
        public async Task<bool> ExistsAsync(long categoryId)
        {
            return await _category.ExistsAsync(x => x.ProductCategoryId == categoryId);
        }

        public async Task<ProductCategoryViewModel> AddAsync(ProductCategoryViewModel model)
        {
            var category = new ProductCategory
            {
               CategoryName = model.CategoryName,
               LastDateModified = DateTime.Now,
               ClientAuthenticationId = model.ClientAuthenticationId,  
               IsDeleted = false,
               Description = model.Description
            };

            await _category.AddAsync(category);

            return _mapper.Map<ProductCategory, ProductCategoryViewModel>(category);
        }

        public async Task UpdateAsync(ProductCategoryViewModel model)
        {
            var entity = await _category.GetSingleAsync(x => x.ProductCategoryId == model.ProductCategoryId);

            entity.CategoryName = model.CategoryName;
            entity.LastDateModified = DateTime.Now;

            await _category.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _category.GetByIdAsync(id);

            await _category.DeleteAsync(entity);
        }

    }

}
