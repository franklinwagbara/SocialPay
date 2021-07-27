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

    public class MerchantPaymentSetupService : IMerchantPaymentSetupService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<MerchantPaymentSetup> _linkSetup;

        public MerchantPaymentSetupService(IAsyncRepository<MerchantPaymentSetup> linkSetup)
        {
            _linkSetup = linkSetup ?? throw new ArgumentNullException(nameof(linkSetup));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantPaymentSetup, PaymentLinkViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<PaymentLinkViewModel>> GetAllAsync()
        {
            var merchants = await _linkSetup.GetAllAsync();

            return _mapper.Map<List<MerchantPaymentSetup>, List<PaymentLinkViewModel>>(merchants);
        }
      
      
        public async Task<PaymentLinkViewModel> GetPaymentLinksId(long storeId)
        {
            var merchant = await _linkSetup.GetSingleAsync(x => x.MerchantStoreId == storeId);

            return _mapper.Map<MerchantPaymentSetup, PaymentLinkViewModel>(merchant);
        }


        public async Task UpdateAsync(PaymentLinkViewModel model)
        {
            var entity = await _linkSetup.GetSingleAsync(x => x.MerchantStoreId == model.MerchantStoreId);

          
            entity.LastDateModified = DateTime.Now;

            await _linkSetup.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _linkSetup.GetByIdAsync(id);

            await _linkSetup.DeleteAsync(entity);
        }

    }

}
