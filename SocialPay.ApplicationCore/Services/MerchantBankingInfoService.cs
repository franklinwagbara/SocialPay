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

    public class MerchantBankingInfoService : IMerchantBankingInfoService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<MerchantBankInfo> _merchantBankInfo;

        public MerchantBankingInfoService(IAsyncRepository<MerchantBankInfo> merchantBankInfo)
        {
            _merchantBankInfo = merchantBankInfo ?? throw new ArgumentNullException(nameof(merchantBankInfo));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantBankInfo, MerchantBankInfoViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<MerchantBankInfoViewModel>> GetAllAsync()
        {
            var merchants = await _merchantBankInfo.GetAllAsync();

            return _mapper.Map<List<MerchantBankInfo>, List<MerchantBankInfoViewModel>>(merchants);
        }

        public async Task<MerchantBankInfoViewModel> GetMerchantBankInfo(long clientId)
        {
            var merchantInfo = await _merchantBankInfo.GetSingleAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<MerchantBankInfo, MerchantBankInfoViewModel>(merchantInfo);
        }

        public async Task<MerchantBankInfoViewModel> GetMerchantByNuban(string nuban)
        {
            var merchantInfo = await _merchantBankInfo.GetSingleAsync(x => x.Nuban == nuban);

            return _mapper.Map<MerchantBankInfo, MerchantBankInfoViewModel>(merchantInfo);
        }

        public async Task<bool> ExistsAsync(long Id)
        {
            return await _merchantBankInfo.ExistsAsync(x => x.MerchantBankInfoId == Id);
        }

        public async Task UpdateAsync(MerchantBankInfoViewModel model)
        {
            var entity = await _merchantBankInfo.GetSingleAsync(x => x.MerchantBankInfoId == model.MerchantBankInfoId);

          
          //  entity.FullName = model.FullName;

            await _merchantBankInfo.UpdateAsync(entity);
        }

        public async Task<int> CountTotalMerchantsAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }


    }

}
