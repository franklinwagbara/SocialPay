using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Extensions.Utilities;
using SocialPay.Core.Messaging;
using SocialPay.Core.Repositories.Customer;
using SocialPay.Helper;
using SocialPay.Helper.Cryptography;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Customer
{
    public class CustomerRepoService
    {
      
        private readonly ICustomerService _customerService;
        private readonly AppSettings _appSettings;
        private readonly EmailService _emailService;
        private readonly EncryptDecryptAlgorithm _encryptDecryptAlgorithm;
        private readonly EncryptDecrypt _encryptDecrypt;
        public CustomerRepoService(ICustomerService customerService, IOptions<AppSettings> appSettings,
            EncryptDecryptAlgorithm encryptDecryptAlgorithm, EncryptDecrypt encryptDecrypt,
            EmailService emailService)
        {
            _customerService = customerService;
            _appSettings = appSettings.Value;
            _encryptDecrypt = encryptDecrypt;
            _encryptDecryptAlgorithm = encryptDecryptAlgorithm;
            _emailService = emailService;
        }

        public async Task<WebApiResponse> GetLinkDetails(string transactionReference)
        {
            try
            {
                //////var decodeUrl = System.Uri.UnescapeDataString(transactionReference);
                //////if (decodeUrl.Contains(" "))
                //////{
                //////    decodeUrl = decodeUrl.Replace(" ", "+");
                //////}
                //////var dd = DecryptAlt(decodeUrl);

               // var decryptedReference = transactionReference.Replace(" ", "+").Decrypt(_appSettings.appKey).Split(",")[3];
                //var res = t1.Decrypt(_appSettings.appKey).Split(",")[3];
                //var decryptedReference1 = transactionReference.Decrypt(_appSettings.appKey);
                //string dv = decryptedReference1.Split(",")[3];
               // var decryptedReference = transactionReference.Decrypt(_appSettings.appKey).Split(",")[3];

                var result = await _customerService.GetTransactionDetails(transactionReference);
                if (result == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPaymentLink};

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetAllCustomerOrders(long clientId)
        {
            try
            {
                return await _customerService.GetCustomerOrders(clientId);
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
                long customerId = 0;
                var getClient = await _customerService.GetClientDetails(model.Email);
                customerId = Convert.ToInt32(getClient.Data);
                var getPaymentDetails = await _customerService.GetTransactionReference(model.TransactionReference);
                if (getPaymentDetails == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPaymentReference };
                if (getClient.ResponseCode != AppResponseCodes.Success)
                {
                    var newCustomerAccess = Guid.NewGuid().ToString("N").Substring(0,10);
                    var createCustomer = await _customerService.CreateNewCustomer(model.Email, newCustomerAccess, model.Fullname,
                       model.PhoneNumber);
                    if (createCustomer.ResponseCode != AppResponseCodes.Success)
                        return new WebApiResponse { ResponseCode = createCustomer.ResponseCode };
                    customerId = Convert.ToInt32(createCustomer.Data);

                    var emailModal = new EmailRequestDto
                    {
                        Subject = "Guest Account Access",
                        SourceEmail = "info@sterling.ng",
                        DestinationEmail = model.Email,
                        // DestinationEmail = "festypat9@gmail.com",
                        //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                    };
                    var mailBuilder = new StringBuilder();
                    mailBuilder.AppendLine("Dear" + " " + model.Email + "," + "<br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("You have successfully sign up as a Guest.<br />");
                    mailBuilder.AppendLine("Kindly use this token" + "  " + newCustomerAccess + "  " + "to login" + " " + "" + "<br />");
                    // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                    mailBuilder.AppendLine("Best Regards,");
                    emailModal.EmailBody = mailBuilder.ToString();

                    var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);
                }
                var encryptedText = _appSettings.mid + _appSettings.paymentCombination + getPaymentDetails.TotalAmount + _appSettings.paymentCombination + Guid.NewGuid().ToString().Substring(0, 10);
                var encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                //var initiatepayment = Process.Start("cmd", "/C start " + _appSettings.sterlingpaymentGatewayRequestUrl + encryptData);
                var paymentData =  _appSettings.sterlingpaymentGatewayRequestUrl + encryptData;
                var paymentResponse = new CustomerResponseDto { CustomerId = customerId, PaymentLink = paymentData };
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> PaymentConfirmation(PaymentValidationRequestDto model)
        {
            try
            {
                var decodeMessage = System.Uri.UnescapeDataString(model.Message);
                if (decodeMessage.Contains(" "))
                {
                    decodeMessage = decodeMessage.Replace(" ", "+");
                }
                var decryptResponse = DecryptAlt(decodeMessage);
                model.Message = decryptResponse;
                var result = await _customerService.LogPaymentResponse(model);
                return result;
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> AcceptOrRejectItem(AcceptRejectRequestDto model, long clientId)
        {
            try
            {
                var validateReference = await _customerService.ValidateShippingRequest(model, clientId);
                return validateReference;
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public static String DecryptAlt(String val)
        {
            MemoryStream ms = new MemoryStream();
            byte[] sharedkey = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11, 0x12, 0x11, 0x0D, 0x0B, 0x07, 0x02, 0x04, 0x08, 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };
            byte[] sharedvector = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toDecrypt = Convert.FromBase64String(val);
            CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);

            cs.Write(toDecrypt, 0, toDecrypt.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
