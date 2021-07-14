using AutoMapper;
using SocialPay.ApplicationCore.Interfaces.Repositories;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Domain.Entities;
using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Services
{

    public class MerchantBusinessInfoService : IMerchantBusinessInfoService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<MerchantBusinessInfo> _merchantBusinessInfo;

        public MerchantBusinessInfoService(IAsyncRepository<MerchantBusinessInfo> merchantBusinessInfo)
        {
            _merchantBusinessInfo = merchantBusinessInfo;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantBusinessInfo, BusinessInfoViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<BusinessInfoViewModel>> GetAllAsync()
        {
            var merchants = await _merchantBusinessInfo.GetAllAsync();

            return _mapper.Map<List<MerchantBusinessInfo>, List<BusinessInfoViewModel>>(merchants);
        }

        public async Task<BusinessInfoViewModel> GetMerchantBusinessInfo(long clientId)
        {
            var merchantInfo = await _merchantBusinessInfo.GetSingleAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<MerchantBusinessInfo, BusinessInfoViewModel>(merchantInfo);
        }

        public async Task<BusinessInfoViewModel> GetMerchantBusinessEmailInfo(string email)
        {
            var merchantInfo = await _merchantBusinessInfo.GetSingleAsync(x => x.BusinessEmail == email);

            return _mapper.Map<MerchantBusinessInfo, BusinessInfoViewModel>(merchantInfo);
        }

        public async Task<BusinessInfoViewModel> GetMerchantBusinessPhoneNumberInfo(string phoneNumber)
        {
            var merchantInfo = await _merchantBusinessInfo.GetSingleAsync(x => x.BusinessPhoneNumber == phoneNumber);

            return _mapper.Map<MerchantBusinessInfo, BusinessInfoViewModel>(merchantInfo);
        }

        public async Task<BusinessInfoViewModel> GetMerchantBusinessTinInfo(string tin)
        {
            var merchantInfo = await _merchantBusinessInfo.GetSingleAsync(x => x.Tin == tin);

            return _mapper.Map<MerchantBusinessInfo, BusinessInfoViewModel>(merchantInfo);
        }

        public async Task<BusinessInfoViewModel> GetMerchantBusinessNameInfo(string businessName)
        {
            var merchantInfo = await _merchantBusinessInfo.GetSingleAsync(x => x.BusinessName == businessName);

            return _mapper.Map<MerchantBusinessInfo, BusinessInfoViewModel>(merchantInfo);
        }
        //GetMerchantBusinessTinInfo
        public async Task<bool> ExistsAsync(long Id)
        {
            return await _merchantBusinessInfo.ExistsAsync(x => x.MerchantBusinessInfoId == Id);
        }

        public async Task UpdateAsync(BusinessInfoViewModel model)
        {
            var entity = await _merchantBusinessInfo.GetSingleAsync(x => x.MerchantBusinessInfoId == model.MerchantBusinessInfoId);

            entity.BusinessEmail = model.BusinessEmail;
            entity.BusinessPhoneNumber = model.BusinessPhoneNumber;
            entity.BusinessName = model.BusinessName;
            entity.Tin = model.Tin;
          //  entity.FullName = model.FullName;

            await _merchantBusinessInfo.UpdateAsync(entity);
        }

        public async Task<int> CountTotalMerchantsAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }


    }

}
