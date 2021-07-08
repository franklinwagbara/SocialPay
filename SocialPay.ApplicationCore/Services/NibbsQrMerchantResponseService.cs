using AutoMapper;
using SocialPay.ApplicationCore.Interfaces.Repositories;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Domain.Entities;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Services
{

    public class NibbsQrMerchantResponseService : INibbsQrMerchantResponseService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<MerchantQRCodeOnboardingResponse> _merchantQRCodeOnboardingResponse;

        public NibbsQrMerchantResponseService(IAsyncRepository<MerchantQRCodeOnboardingResponse> merchantQRCodeOnboardingResponse)
        {
            _merchantQRCodeOnboardingResponse = merchantQRCodeOnboardingResponse;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantQRCodeOnboardingResponse, CreateNibsMerchantQrCodeResponse>());

            _mapper = config.CreateMapper();
        }
              
        public async Task<CreateNibsMerchantQrCodeResponse> GetMerchantInfo(long clientId)
        {
            var merchant = await _merchantQRCodeOnboardingResponse.GetSingleAsync(x => x.MerchantQRCodeOnboardingId == clientId);

            return _mapper.Map<MerchantQRCodeOnboardingResponse, CreateNibsMerchantQrCodeResponse>(merchant);
        }              

        public async Task<bool> ExistsAsync(long clientId)
        {
            return await _merchantQRCodeOnboardingResponse.ExistsAsync(x => x.MerchantQRCodeOnboardingResponseId == clientId);
        }

        public async Task<int> CountTotalMerchantsAsync()
        {
            return 1;
           // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _merchantQRCodeOnboardingResponse.GetByIdAsync(id);

            await _merchantQRCodeOnboardingResponse.DeleteAsync(entity);
        }        

    }

}
