using AutoMapper;
using SocialPay.ApplicationCore.Interfaces.Repositories;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Domain.Entities;
using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Services
{

    public class MerchantTransactionSetupService : IMerchantTransactionSetup
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<MerchantTransactionSetup> _merchantTransactionSetup;

        public MerchantTransactionSetupService(IAsyncRepository<MerchantTransactionSetup> merchantTransactionSetup)
        {
            _merchantTransactionSetup = merchantTransactionSetup;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantTransactionSetup, MerchantTransactionSetupViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<MerchantTransactionSetupViewModel>> GetAllAsync()
        {
            var merchants = await _merchantTransactionSetup.GetAllAsync();

            return _mapper.Map<List<MerchantTransactionSetup>, List<MerchantTransactionSetupViewModel>>(merchants);
        }

        public async Task<MerchantTransactionSetupViewModel> GetMerchantInfo(long clientId)
        {
            var merchantInfo = await _merchantTransactionSetup.GetSingleAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<MerchantTransactionSetup, MerchantTransactionSetupViewModel>(merchantInfo);
        }

        public async Task<MerchantTransactionSetupViewModel> GetMerchantValidationInfo(long clientId, string pin)
        {
            var merchantInfo = await _merchantTransactionSetup
                .GetSingleAsync(x => x.ClientAuthenticationId == clientId
                && x.Pin == pin);

            return _mapper.Map<MerchantTransactionSetup, MerchantTransactionSetupViewModel>(merchantInfo);
        }

        public async Task<MerchantTransactionSetupViewModel> AddAsync(MerchantTransactionSetupViewModel model)
        {
            var entity = new MerchantTransactionSetup
            {
               ClientAuthenticationId = model.ClientAuthenticationId,
               LastDateModified = model.LastDateModified,
               Pin = model.Pin,
               Status = model.Status
            };

            await _merchantTransactionSetup.AddAsync(entity);

            return _mapper.Map<MerchantTransactionSetup, MerchantTransactionSetupViewModel>(entity);
        }


        public async Task<bool> ExistsAsync(long Id)
        {
            return await _merchantTransactionSetup.ExistsAsync(x => x.ClientAuthenticationId == Id);
        }

        public async Task UpdateAsync(MerchantTransactionSetupViewModel model)
        {
            var entity = await _merchantTransactionSetup.GetSingleAsync(x => x.MerchantTransactionSetupId == model.MerchantTransactionSetupId);

            
          //  entity.FullName = model.FullName;

            await _merchantTransactionSetup.UpdateAsync(entity);
        }

        public async Task<int> CountTotalMerchantsAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }


    }

}
