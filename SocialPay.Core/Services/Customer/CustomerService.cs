using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Repositories.Customer;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Customer
{
    public class CustomerRepoService
    {
      
        private readonly ICustomerService _customerService;
        private readonly AppSettings _appSettings;
        public CustomerRepoService(ICustomerService customerService, IOptions<AppSettings> appSettings)
        {
            _customerService = customerService;
            _appSettings = appSettings.Value;
        }

        public async Task<WebApiResponse> GetLinkDetails(string transactionReference)
        {
            try
            {

                var decryptedReference = transactionReference.Decrypt(_appSettings.appKey).Split(",")[3];
                var result = await _customerService.GetTransactionDetails(decryptedReference);

                if (result == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPaymentLink};

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> MakePayment(CustomerPaymentRequestDto model)
        {
            try
            {

                var getClient = await _customerService.GetClientDetails(model.Email);
                if(getClient == null)
                {
                    //var createCustomer = await _customerService.CreateNewCustomer(model.Email, model.Fullname,
                    //    model.PhoneNumber);
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
