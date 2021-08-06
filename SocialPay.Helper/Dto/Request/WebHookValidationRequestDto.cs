namespace SocialPay.Helper.Dto.Request
{
    public class WebHookValidationRequestDto
    {
        public string notificationType { get; set; }
        public string timeStamp { get; set; }
        public string merchantName { get; set; }
        public string merchantNo { get; set; }
        public string subMerchantName { get; set; }
        public string subMerchantNo { get; set; }
        public string transactionTime { get; set; }
        public string transactionAmount { get; set; }
        public string merchantFee { get; set; }
        public string residualAmount { get; set; }
        public string transactionType { get; set; }
        public string orderSn { get; set; }
        public string orderNo { get; set; }
        public string sign { get; set; }
    }
}
