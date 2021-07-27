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

    public class StoreService : IStoreService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<MerchantStore> _store;

        public StoreService(IAsyncRepository<MerchantStore> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantStore, StoreViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<StoreViewModel>> GetAllAsync()
        {
            var stores = await _store.GetAllAsync();

            return _mapper.Map<List<MerchantStore>, List<StoreViewModel>>(stores);
        }

        public async Task<List<StoreViewModel>> GetStoresByClientId(long clientId)
        {
            var stores = await _store.GetAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<List<MerchantStore>, List<StoreViewModel>>(stores);
        }

        public async Task<StoreViewModel> GetStoreById(long storeId)
        {
            var store = await _store.GetSingleAsync(x => x.MerchantStoreId == storeId);

            return _mapper.Map<MerchantStore, StoreViewModel>(store);
        }

        public async Task<StoreViewModel> GetStoreName(string storeName)
        {
            var store = await _store.GetSingleAsync(x => x.StoreName == storeName);

            return _mapper.Map<MerchantStore, StoreViewModel>(store);
        }
        public async Task<StoreViewModel> GetStoreById(long storeId, long clientId)
        {
            var store = await _store.GetSingleAsync(x => x.MerchantStoreId == storeId && x.ClientAuthenticationId == clientId);

            return _mapper.Map<MerchantStore, StoreViewModel>(store);
        }

        public async Task<bool> ExistsAsync(long clientId)
        {
            return await _store.ExistsAsync(x => x.ClientAuthenticationId == clientId);
        }

        public async Task<StoreViewModel> AddAsync(StoreViewModel model)
        {
            var store = new MerchantStore
            {
                StoreName = model.StoreName,
                ClientAuthenticationId = model.ClientAuthenticationId,
                Description = model.Description,
                LastDateModified = DateTime.Now,
                FileLocation = model.FileLocation,
                Image = model.Image,
                StoreLink = model.StoreLink
            };

            await _store.AddAsync(store);

            return _mapper.Map<MerchantStore, StoreViewModel>(store);
        }

        public async Task UpdateAsync(StoreViewModel model)
        {
            var entity = await _store.GetSingleAsync(x => x.MerchantStoreId == model.MerchantStoreId);

            entity.Description = model.Description;
            entity.StoreName = model.StoreName;
            entity.LastDateModified = DateTime.Now;

            await _store.UpdateAsync(entity);
        }

        public async Task<int> CountTotalStoressAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _store.GetByIdAsync(id);

            await _store.DeleteAsync(entity);
        }

    }

}
