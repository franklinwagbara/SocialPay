using System;

namespace SocialPay.Helper.Dto.Response
{
    public class WebApiResponse
    {
        public string ResponseCode { get; set; }
        public string UserStatus { get; set; }
        public Object Data { get; set; }
    }


    public class InitiatePaymentResponse
    {
        public string ResponseCode { get; set; }
        public string PaymentRef { get; set; }
        public Object Data { get; set; }
    }

    public class LoginAPIResponse
    {
        public string ClientId { get; set; }
        public string ResponseCode { get; set; }
        public string UserStatus { get; set; }
        public string AccessToken { get; set; }
        public string Role { get; set; }
        public string BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string BankName { get; set; }
        public string Nuban { get; set; }
        public string AccountName { get; set; }
        public double MerchantWalletBalance { get; set; }
    }
}
