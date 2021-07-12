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

    public class NibbsQrMerchantService : INibbsQrMerchantService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<MerchantQRCodeOnboarding> _merchantQRCodeOnboarding;

        public NibbsQrMerchantService(IAsyncRepository<MerchantQRCodeOnboarding> merchantQRCodeOnboarding)
        {
            _merchantQRCodeOnboarding = merchantQRCodeOnboarding;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantQRCodeOnboarding, NibbsQrMerchantViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<NibbsQrMerchantViewModel>> GetAllAsync()
        {
            var merchants = await _merchantQRCodeOnboarding.GetAllAsync();

            return _mapper.Map<List<MerchantQRCodeOnboarding>, List<NibbsQrMerchantViewModel>>(merchants);
        }

        public async Task<NibbsQrMerchantViewModel> GetMerchantInfo(long clientId)
        {
            var merchant = await _merchantQRCodeOnboarding.GetSingleAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<MerchantQRCodeOnboarding, NibbsQrMerchantViewModel>(merchant);
        }

        public async Task<NibbsQrMerchantViewModel> GetMerchantStatusInfo(long clientId, string status)
        {
            var merchant = await _merchantQRCodeOnboarding.GetSingleAsync(x => x.ClientAuthenticationId == clientId && x.Status == status);

            return _mapper.Map<MerchantQRCodeOnboarding, NibbsQrMerchantViewModel>(merchant);
        }

        public async Task<bool> ExistsAsync(long clientId)
        {
            return await _merchantQRCodeOnboarding.ExistsAsync(x => x.ClientAuthenticationId == clientId && x.IsCompleted == true);
        }

        public async Task<bool> ExistsAsync(long clientId, string status)
        {
            return await _merchantQRCodeOnboarding.ExistsAsync(x => x.ClientAuthenticationId == clientId && x.Status == status);
        }

        public async Task AddAsync(NibbsQrMerchantViewModel model)
        {
            var entity = new MerchantQRCodeOnboarding
            {
               IsDeleted = false,
               Address = model.Address,
               Contact = model.Contact,
               Email = model.Email,
               Fee = model.Fee,
               ClientAuthenticationId = model.ClientAuthenticationId,
               Name = model.Name,
               Phone = model.Phone,
               Tin = model.Tin,
               LastDateModified = DateTime.Now
            };

            await _merchantQRCodeOnboarding.AddAsync(entity);
            // return _mapper.Map<SubmitOtpRequest, SubmitOtp>(entity);
          //  return null;
        }

        public async Task UpdateAsync(NibbsQrMerchantViewModel model)
        {
            var entity = await _merchantQRCodeOnboarding.GetSingleAsync(x => x.ClientAuthenticationId == model.ClientAuthenticationId);

            entity.Email = model.Email;
            entity.Address = model.Address;
            entity.Contact = model.Contact;
            entity.Fee = model.Fee;

            await _merchantQRCodeOnboarding.UpdateAsync(entity);
        }       

        public async Task<int> CountTotalMerchantsAsync()
        {
            return 1;
           // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _merchantQRCodeOnboarding.GetByIdAsync(id);

            await _merchantQRCodeOnboarding.DeleteAsync(entity);
        }        

    }

}
