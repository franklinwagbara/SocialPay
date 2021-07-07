namespace SocialPay.Helper.Dto.Response
{
    public class CreateNibsMerchantQrCodeResponse
    {
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
        public string mchNo { get; set; }
        public string merchantName { get; set; }
        public string merchantTIN { get; set; }
        public string merchantAddress { get; set; }
        public string merchantContactName { get; set; }
        public string merchantPhoneNumber { get; set; }
        public string merchantEmail { get; set; }
        public string ResponseCode { get; set; }
        public string jsonResponse { get; set; }
    }
}
