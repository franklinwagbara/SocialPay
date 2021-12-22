using System;

namespace SocialPay.Helper.Dto.Response
{
    public class WebApiResponse
    {
        public string ResponseCode { get; set; }
        public string UserStatus { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public Object Data { get; set; }
    }

    public class InitiatePayUPaymentResponse
    {
        public string merchantReference { get; set; }
        public string payUVasReference { get; set; }
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
        public string pointOfFailure { get; set; }
        public string merchantId { get; set; }
        public Object DataObj { get; set; }

    }

    public class InitiatePaymentResponse
    {
        public string ResponseCode { get; set; }
        public string PaymentRef { get; set; }
        public string TransactionReference { get; set; }
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
        public string BankCode { get; set; }
        public string Nuban { get; set; }
        public string AccountName { get; set; }
        public string Refcode { get; set; }
        public string QRStatus { get; set; }
        public string Message { get; set; }
        public double MerchantWalletBalance { get; set; }
        public bool HasBusinessInfo { get; set; }
    }
}
