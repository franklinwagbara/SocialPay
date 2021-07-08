using AutoMapper;
using SocialPay.ApplicationCore.Interfaces.Repositories;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Domain.Entities;
using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Services
{

    public class NibbsQrSubMerchantService : INibbsQrSubMerchantService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<SubMerchantQRCodeOnboarding> _subMerchantQRCodeOnboarding;

        public NibbsQrSubMerchantService(IAsyncRepository<SubMerchantQRCodeOnboarding> subMerchantQRCodeOnboarding)
        {
            _subMerchantQRCodeOnboarding = subMerchantQRCodeOnboarding;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubMerchantQRCodeOnboarding, NibbsSubMerchantViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<NibbsSubMerchantViewModel>> GetAllAsync()
        {
            var merchants = await _subMerchantQRCodeOnboarding.GetAllAsync();

            return _mapper.Map<List<SubMerchantQRCodeOnboarding>, List<NibbsSubMerchantViewModel>>(merchants);
        }

        public async Task<NibbsSubMerchantViewModel> GetMerchantInfo(long Id)
        {
            var merchant = await _subMerchantQRCodeOnboarding.GetSingleAsync(x => x.SubMerchantQRCodeOnboardingId == Id);

            return _mapper.Map<SubMerchantQRCodeOnboarding, NibbsSubMerchantViewModel>(merchant);
        }              

        public async Task<bool> ExistsAsync(long Id)
        {
            return await _subMerchantQRCodeOnboarding.ExistsAsync(x => x.SubMerchantQRCodeOnboardingId == Id);
        }

        public async Task AddAsync(NibbsSubMerchantViewModel model)
        {
            var entity = new SubMerchantQRCodeOnboarding
            {
              
            };

            await _subMerchantQRCodeOnboarding.AddAsync(entity);
            // return _mapper.Map<SubmitOtpRequest, SubmitOtp>(entity);
          //  return null;
        }

        public async Task UpdateAsync(NibbsSubMerchantViewModel model)
        {
            var entity = await _subMerchantQRCodeOnboarding.GetSingleAsync(x => x.SubMerchantQRCodeOnboardingId == model.SubMerchantQRCodeOnboardingId);

            //entity.Email = model.Email;
            //entity.Address = model.Address;
            //entity.Contact = model.Contact;
            //entity.Fee = model.Fee;

            await _subMerchantQRCodeOnboarding.UpdateAsync(entity);
        }       

        public async Task<int> CountTotalMerchantsAsync()
        {
            return 1;
           // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _subMerchantQRCodeOnboarding.GetByIdAsync(id);

            await _subMerchantQRCodeOnboarding.DeleteAsync(entity);
        }        

    }

}
