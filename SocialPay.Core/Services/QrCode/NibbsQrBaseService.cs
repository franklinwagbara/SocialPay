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
        private readonly IClientAuthenticationService _clientAuthenticationService;
        // private readonly NibbsQRCodeAPIService _nibbsQRCodeAPIService;
        private readonly MerchantPersonalInfoRepository _merchantPersonalInfoRepository;
        private readonly NibbsQrRepository _nibbsQrRepository;
        public NibbsQrBaseService(INibbsQrMerchantService nibbsQrMerchantService, NibbsQrRepository nibbsQrRepository,
            MerchantPersonalInfoRepository merchantPersonalInfoRepository, INibbsQrSubMerchantService nibbsQrSubMerchantService,
            INibbsQrMerchantResponseService nibbsQrMerchantResponseService,
            IClientAuthenticationService clientAuthenticationService)
        {
            _nibbsQrMerchantService = nibbsQrMerchantService ?? throw new ArgumentNullException(nameof(nibbsQrMerchantService));
            _nibbsQrRepository = nibbsQrRepository ?? throw new ArgumentNullException(nameof(nibbsQrRepository));
            _merchantPersonalInfoRepository = merchantPersonalInfoRepository ?? throw new ArgumentNullException(nameof(merchantPersonalInfoRepository));
            _nibbsQrSubMerchantService = nibbsQrSubMerchantService ?? throw new ArgumentNullException(nameof(nibbsQrSubMerchantService));
            _nibbsQrMerchantResponseService = nibbsQrMerchantResponseService ?? throw new ArgumentNullException(nameof(nibbsQrMerchantResponseService));
            _clientAuthenticationService = clientAuthenticationService ?? throw new ArgumentNullException(nameof(clientAuthenticationService));
        }

        public async Task<WebApiResponse> CreateMerchantAsync(DefaultMerchantRequestDto requestDto, long clientId)
        {

            //clientId = 90;

            if (await _nibbsQrMerchantService.ExistsAsync(clientId))
                return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails, Data = "Duplicate Merchant" };

           // var merchant = await _merchantPersonalInfoRepository.GetCompleteMerchantPersonalDetailsAsync(clientId);
            var merchant = await _clientAuthenticationService.GetUserByClientIdInfo(clientId);

            if (merchant == null)
                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

            var model = new NibbsQrMerchantViewModel
            {
                ClientAuthenticationId = clientId,
                IsDeleted = false,
                Address = requestDto.Address,
                Contact = requestDto.Contact,
                Email = merchant.Email,
                Fee = 00,
                Name = merchant.FullName,
                Phone = merchant.PhoneNumber
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
                var merchant = await _nibbsQrMerchantService.GetMerchantStatusInfo(clientId, NibbsMerchantOnboarding.BindMerchant);

                var clientInfo = await _clientAuthenticationService.GetUserByClientIdInfo(clientId);

                if (clientInfo.QrCodeStatus != NibbsMerchantOnboarding.BindMerchant)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.QRMerchantOnboardingNotFoundOrCompleted, Message = "QR Merchant onboarding not found/completed" };

                var qrInfo = await _nibbsQrMerchantResponseService.GetMerchantInfo(merchant.MerchantQRCodeOnboardingId);
                //var merchant = await _nibbsQrMerchantService

                var defaultRequest = new DynamicPaymentDefaultRequestDto
                {
                    amount = request.amount,
                    customerIdentifier = merchant.Email,
                    orderNo = request.orderNo,
                    userAccountName = qrInfo.merchantName,
                    userAccountNumber = qrInfo.mchNo,
                    userBankVerificationNumber = clientInfo.Bvn,
                    userGps = request.userGps,
                    userKycLevel = "1"
                };

                return await _nibbsQrRepository.QrDynamicPayAsync(defaultRequest, clientId);
                // await _nibbsQrMerchantService.AddAsync(model);

               // return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Merchant was successfully created" };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while creating merchant" };
            }
        }


    }
}
