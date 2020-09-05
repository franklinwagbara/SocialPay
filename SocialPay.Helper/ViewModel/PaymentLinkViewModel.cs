namespace SocialPay.Helper.ViewModel
{
    public class PaymentLinkViewModel
    {
        public string PaymentLinkName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string CustomUrl { get; set; }
        public string DeliveryMethod { get; set; }
        public string DeliveryTime { get; set; }
        public string TransactionReference { get; set; }
    }
}
