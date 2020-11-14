using System;

namespace SocialPay.Helper.ViewModel
{
    public class EscrowViewModel
    {
        public string PaymentLinkName { get; set; }
        public string MerchantDescription { get; set; }
        public string CustomerDescription { get; set; }
        public decimal MerchantAmount { get; set; }
        public decimal CustomerAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AdditionalCharges { get; set; }
        public bool HasAdditionalCharges { get; set; }
        public string CustomUrl { get; set; }
        public string DeliveryMethod { get; set; }
        public long DeliveryTime { get; set; }
        public bool RedirectAfterPayment { get; set; }
        public string AdditionalDetails { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentLinkUrl { get; set; }
        public string Document { get; set; }
        public string CustomerEmail { get; set; }
        public string Channel { get; set; }
        public string OrderStatus { get; set; }
        public string Message { get; set; }
        public long ClientId { get; set; }
        public string CustomerTransactionReference { get; set; }
    }

    public class ItemDisputeViewModel
    {
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string Comment { get; set; }
        public string Document { get; set; }
        public DateTime DateEntered { get; set; }
    }
}
