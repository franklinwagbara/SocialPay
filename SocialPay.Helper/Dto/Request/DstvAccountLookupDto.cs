namespace SocialPay.Helper.Dto.Request
{
    public class DstvAccountLookupDto
    {
        public string merchantReference { get; set; }
        public string transactionType { get; set; }
        public string vasId { get; set; }
        public string countryCode { get; set; }
        public string customerId { get; set; }
    }
}
