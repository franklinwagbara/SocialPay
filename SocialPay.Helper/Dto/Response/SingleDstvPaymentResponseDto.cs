namespace SocialPay.Helper.Dto.Response
{
    public class SingleDstvPaymentResponseDto
    {
        public long SingleDstvPaymentResponseId { get; set; }
        public string merchantReference { get; set; }
        public string payUVasReference { get; set; }
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
        public string pointOfFailure { get; set; }
        public string merchantId { get; set; }
    }
}
