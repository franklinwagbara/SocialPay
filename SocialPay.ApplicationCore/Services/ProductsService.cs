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

    public class ProductsService : IProductsService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<Product> _products;

        public ProductsService(IAsyncRepository<Product> products)
        {
            _products = products;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductsViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<ProductsViewModel>> GetAllAsync()
        {
            var products = await _products.GetAllAsync();

            return _mapper.Map<List<Product>, List<ProductsViewModel>>(products);
        }

        public async Task<List<ProductsViewModel>> GetProductsByClientId(long clientId)
        {
            var products = await _products.GetAsync(x => x.ProductCategoryId == clientId);

            return _mapper.Map<List<Product>, List<ProductsViewModel>>(products);
        }

        public async Task<ProductsViewModel> GetProductByNameCatIdAndClientId(string productName, long categoryId, long storeId)
        {
            var product = await _products
                .GetSingleAsync(x => x.ProductName == productName
                && x.ProductCategoryId == categoryId
                && x.StoreId == storeId);

            return _mapper.Map<Product, ProductsViewModel>(product);
        }

        public async Task<ProductsViewModel> GetProductById(long productId)
        {
            var product = await _products.GetSingleAsync(x => x.ProductId == productId);

            return _mapper.Map<Product, ProductsViewModel>(product);
        }

        public async Task<bool> ExistsAsync(long productId)
        {
            return await _products.ExistsAsync(x => x.ProductId == productId);
        }

        public async Task<ProductsViewModel> AddAsync(ProductsViewModel model)
        {
            var product = new Product
            {
              Description = model.Description,
              LastDateModified = DateTime.Now,
              Color = model.Color,
              Options = model.Options,
              Price = model.Price,
              ProductCategoryId = model.ProductCategoryId,
              ProductName = model.ProductName,
              Size = model.Size,
              ProductReference = model.ProductReference,
              IsDeleted = false              
            };

            await _products.AddAsync(product);

            return _mapper.Map<Product, ProductsViewModel>(product);
        }

        public async Task UpdateAsync(ProductsViewModel model)
        {
            var entity = await _products.GetSingleAsync(x => x.ProductId == model.ProductId);

            entity.Description = model.Description;
            entity.ProductName = model.ProductName;
            entity.LastDateModified = DateTime.Now;

            await _products.UpdateAsync(entity);
        }

        public async Task<int> CountTotalProductsAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _products.GetByIdAsync(id);

            await _products.DeleteAsync(entity);
        }

    }

}
