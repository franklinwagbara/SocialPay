using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Extensions.Utilities;
using SocialPay.Core.Repositories.Customer;
using SocialPay.Helper;
using SocialPay.Helper.Cryptography;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Customer
{
    public class CustomerRepoService
    {
      
        private readonly ICustomerService _customerService;
        private readonly AppSettings _appSettings;
        private readonly EncryptDecryptAlgorithm _encryptDecryptAlgorithm;
        private readonly EncryptDecrypt _encryptDecrypt;
        public CustomerRepoService(ICustomerService customerService, IOptions<AppSettings> appSettings,
            EncryptDecryptAlgorithm encryptDecryptAlgorithm, EncryptDecrypt encryptDecrypt)
        {
            _customerService = customerService;
            _appSettings = appSettings.Value;
            _encryptDecrypt = encryptDecrypt;
            _encryptDecryptAlgorithm = encryptDecryptAlgorithm;
        }

        public async Task<WebApiResponse> GetLinkDetails(string transactionReference)
        {
            try
            {
                var decryptedReference = transactionReference.Replace(" ", "+").Decrypt(_appSettings.appKey).Split(",")[3];
                //var res = t1.Decrypt(_appSettings.appKey).Split(",")[3];
                //var decryptedReference1 = transactionReference.Decrypt(_appSettings.appKey);
                //string dv = decryptedReference1.Split(",")[3];
               // var decryptedReference = transactionReference.Decrypt(_appSettings.appKey).Split(",")[3];

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
                var getPaymentDetails = await _customerService.GetTransactionReference(model.TransactionReference);
                if (getPaymentDetails == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPaymentReference };
                if (getClient.ResponseCode != AppResponseCodes.Success)
                {
                    var createCustomer = await _customerService.CreateNewCustomer(model.Email, model.Fullname,
                       model.PhoneNumber);
                    if (createCustomer.ResponseCode != AppResponseCodes.Success)
                        return new WebApiResponse { ResponseCode = createCustomer.ResponseCode };
                }
                   
                var encryptedText = _appSettings.mid + _appSettings.paymentCombination + getPaymentDetails.Amount + _appSettings.paymentCombination + Guid.NewGuid().ToString().Substring(0, 10);
                var encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                //var initiatepayment = Process.Start("cmd", "/C start " + _appSettings.sterlingpaymentGatewayRequestUrl + encryptData);
                var paymentData =  _appSettings.sterlingpaymentGatewayRequestUrl + encryptData;
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = paymentData };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
