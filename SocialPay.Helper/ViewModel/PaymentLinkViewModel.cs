using System;

namespace SocialPay.Helper.ViewModel
{
    public class PaymentLinkViewModel
    {
        public string PaymentLinkName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomUrl { get; set; }
        public string DeliveryMethod { get; set; }
        public long DeliveryTime { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentLinkUrl { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMethod { get; set; }
        public string AdditionalDetails { get; set; }
    }

    public class CustomerPaymentViewModel
    {
        public string Description { get; set; }
        public string CustomerDescription { get; set; }
        public decimal MerchantAmount { get; set; }
        public decimal CustomerAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryMethod { get; set; }
        public long DeliveryTime { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class OrdersViewModel
    {
        public long CustomerTransactionId { get; set; }
        public string Description { get; set; }
        public decimal MerchantAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryMethod { get; set; }
        public DateTime DeliveryTime { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentLinkUrl { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMethod { get; set; }
        public string AdditionalDetails { get; set; }
        public string OrderStatus { get; set; }
        public long RequestId { get; set; }
    }
}
