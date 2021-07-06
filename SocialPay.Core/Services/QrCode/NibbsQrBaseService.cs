using SocialPay.ApplicationCore.Interfaces.Service;
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

        public NibbsQrBaseService(INibbsQrMerchantService nibbsQrMerchantService)
        {
            _nibbsQrMerchantService = nibbsQrMerchantService ?? throw new ArgumentNullException(nameof(nibbsQrMerchantService));
        }

        public async Task<WebApiResponse> CreateMerchantAsync(CreateNibsMerchantRequestDto requestDto, long clientId)
        {
            try
            {
                var model = new NibbsQrMerchantViewModel
                {
                    ClientAuthenticationId = clientId,
                    IsDeleted = false,
                    Address = requestDto.Address,
                    Contact = requestDto.Contact,
                    Email = requestDto.Email,
                    Fee = requestDto.Fee,
                    Name = requestDto.Name,
                    Phone = requestDto.Phone,
                    Tin = requestDto.Tin
                };

                await _nibbsQrMerchantService.AddAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Merchant was successfully created" };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while creating merchant" };
            }
        }
    }
}
