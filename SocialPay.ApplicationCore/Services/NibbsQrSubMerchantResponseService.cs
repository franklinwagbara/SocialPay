using AutoMapper;
using SocialPay.ApplicationCore.Interfaces.Repositories;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Domain.Entities;
using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Services
{

    public class NibbsQrSubMerchantResponseService : INibbsQrSubMerchantResponseService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<SubMerchantQRCodeOnboardingResponse> _subMerchantQRCodeOnboardingResponse;

        public NibbsQrSubMerchantResponseService(IAsyncRepository<SubMerchantQRCodeOnboardingResponse> subMerchantQRCodeOnboardingResponse)
        {
            _subMerchantQRCodeOnboardingResponse = subMerchantQRCodeOnboardingResponse;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubMerchantQRCodeOnboardingResponse, SubMerchantQrResponseViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<SubMerchantQrResponseViewModel>> GetAllAsync()
        {
            var merchants = await _subMerchantQRCodeOnboardingResponse.GetAllAsync();

            return _mapper.Map<List<SubMerchantQRCodeOnboardingResponse>, List<SubMerchantQrResponseViewModel>>(merchants);
        }

        public async Task<SubMerchantQrResponseViewModel> GetMerchantInfo(long clientId)
        {
            var merchant = await _subMerchantQRCodeOnboardingResponse.GetSingleAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<SubMerchantQRCodeOnboardingResponse, SubMerchantQrResponseViewModel>(merchant);
        }              

    }

}
