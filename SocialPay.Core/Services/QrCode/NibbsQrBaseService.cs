using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Core.Services.Merchant;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.QrCode
{
    public class NibbsQrBaseService
    {
        private readonly INibbsQrMerchantService _nibbsQrMerchantService;
        private readonly INibbsQrSubMerchantService _nibbsQrSubMerchantService;
        private readonly INibbsQrMerchantResponseService _nibbsQrMerchantResponseService;
        // private readonly NibbsQRCodeAPIService _nibbsQRCodeAPIService;
        private readonly MerchantPersonalInfoRepository _merchantPersonalInfoRepository;
        private readonly NibbsQrRepository _nibbsQrRepository;
        public NibbsQrBaseService(INibbsQrMerchantService nibbsQrMerchantService, NibbsQrRepository nibbsQrRepository,
            MerchantPersonalInfoRepository merchantPersonalInfoRepository, INibbsQrSubMerchantService nibbsQrSubMerchantService,
            INibbsQrMerchantResponseService nibbsQrMerchantResponseService)
        {
            _nibbsQrMerchantService = nibbsQrMerchantService ?? throw new ArgumentNullException(nameof(nibbsQrMerchantService));
            _nibbsQrRepository = nibbsQrRepository ?? throw new ArgumentNullException(nameof(nibbsQrRepository));
            _merchantPersonalInfoRepository = merchantPersonalInfoRepository ?? throw new ArgumentNullException(nameof(merchantPersonalInfoRepository));
            _nibbsQrSubMerchantService = nibbsQrSubMerchantService ?? throw new ArgumentNullException(nameof(nibbsQrSubMerchantService));
            _nibbsQrMerchantResponseService = nibbsQrMerchantResponseService ?? throw new ArgumentNullException(nameof(nibbsQrMerchantResponseService));
        }

        public async Task<WebApiResponse> CreateMerchantAsync(DefaultMerchantRequestDto requestDto, long clientId)
        {

            //clientId = 90;

            if (await _nibbsQrMerchantService.ExistsAsync(clientId))
                return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails, Data = "Duplicate Merchant" };

            var merchant = await _merchantPersonalInfoRepository.GetCompleteMerchantPersonalDetailsAsync(clientId);

            if (merchant.ResponseCode != AppResponseCodes.Success)
                return new WebApiResponse { ResponseCode = merchant.ResponseCode };

            var model = new NibbsQrMerchantViewModel
            {
                ClientAuthenticationId = clientId,
                IsDeleted = false,
                Address = requestDto.Address,
                Contact = requestDto.Contact,
                Email = merchant.BusinessEmail,
                Fee = 00,
                Name = merchant.BusinessName,
                Phone = merchant.BusinessPhoneNumber,
                Tin = merchant.Tin
            };

            return await _nibbsQrRepository.CreateMerchantAsync(model, clientId);
        }

        public async Task<WebApiResponse> CreateSubMerchantAsync(DefaultSubMerchantRequestDto requestDto, long clientId)
        {
            //clientId = 90;
            try
            {
                var merchant = await _nibbsQrMerchantService.GetMerchantStatusInfo(clientId, NibbsMerchantOnboarding.CreateAccount);

                var merchantResponseInfo = await _nibbsQrMerchantResponseService.GetMerchantInfo(merchant.MerchantQRCodeOnboardingId);

                if (merchantResponseInfo == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                var model = new NibbsSubMerchantViewModel
                {
                   mchNo = merchantResponseInfo.mchNo,
                   merchantEmail = merchantResponseInfo.merchantEmail,
                   merchantName = merchantResponseInfo.merchantName,
                   merchantPhoneNumber = merchantResponseInfo.merchantPhoneNumber,
                   subAmount = requestDto.subAmount,
                   subFixed = requestDto.subFixed,
                   MerchantQRCodeOnboardingId = merchant.MerchantQRCodeOnboardingId
                };

                return await _nibbsQrRepository.CreateSubMerchantAsync(model, clientId);

               // return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Merchant was successfully created" };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while creating merchant" };
            }
        }

        public async Task<WebApiResponse> BindMerchantAsync(long clientId)
        {
           // clientId = 90;

            try
            {
                //var merchant = await _nibbsQrMerchantService.GetMerchantInfo(clientId);

                ////var getMerchantDetails = await _nibbsQrSubMerchantService.GetMerchantInfo(merchant.MerchantQRCodeOnboardingId);

                //var request = new BindMerchantRequestDto
                //{
                   
                //};

                return await _nibbsQrRepository.BindMerchantAync(clientId);
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while creating merchant" };
            }
        }

        public async Task<WebApiResponse> DynamicPaymentAsync(DynamicPaymentRequestDto request, long clientId)
        {
            try
            {
               

                var model = new NibbsSubMerchantViewModel
                {
                    // ClientAuthenticationId = clientId,
                };


                // await _nibbsQrMerchantService.AddAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Merchant was successfully created" };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while creating merchant" };
            }
        }


    }
}
