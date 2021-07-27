using System;

namespace SocialPay.Helper.ViewModel
{
    public class PaymentLinkViewModel
    {
        public MerchantInfoViewModel MerchantInfo { get; set; }
        public string PaymentLinkName { get; set; }
        public long MerchantPaymentSetupId { get; set; }
        public long MerchantStoreId { get; set; }
        public string MerchantDescription { get; set; }
        public string CustomerDescription { get; set; }
        public decimal MerchantAmount { get; set; }
        public decimal CustomerAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomUrl { get; set; }
        public string DeliveryMethod { get; set; }
        public long DeliveryTime { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentLinkUrl { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMethod { get; set; }
        public string MerchantDocument { get; set; }
        public string AdditionalDetails { get; set; }
    }

    public class CustomerPaymentViewModel
    {
        public long CustomerTransactionId { get; set; }
        public string MerchantDescription { get; set; }
        public string CustomerDescription { get; set; }
        public decimal MerchantAmount { get; set; }
        public decimal CustomerAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryMethod { get; set; }
        public long DeliveryTime { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string Document { get; set; }
        public string Fullname { get; set; }
        public DateTime TransactionDate { get; set; }
    }


    public class CustomerInfoViewModel
    {
        public long ClientAuthenticationId { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string Fullname { get; set; }
        //public string PaymentLinkUrl { get; set; }
        public DateTime DateRegistered { get; set; } 
    }

    public class OrdersViewModel
    {
        public long CustomerTransactionId { get; set; }
        public string MerchantDescription { get; set; }
        public string CustomerDescription { get; set; }
        public decimal MerchantAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryMethod { get; set; }
        public DateTime DeliveryTime { get; set; }
        public string TransactionReference { get; set; }
        public string TransactionStatus { get; set; }
        public string PaymentLinkUrl { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionDate { get; set; }
        public string AdditionalDetails { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentReference { get; set; }
        public string MerchantName { get; set; }
        public string CustomerName { get; set; }
        public long RequestId { get; set; }
        public long ClientId { get; set; }
        public string CustomerTransactionReference { get; set; }
    }

    public class MerchantInfoViewModel
    {
        public string BusinessName { get; set; }
        public string BusinessPhoneNumber { get; set; }
        public string BusinessEmail { get; set; }
        public string Country { get; set; }
        public bool HasSpectaMerchantID { get; set; }
        public string Chargebackemail { get; set; }
        public string Logo { get; set; }
     
    }

    public class InvoiceViewModel
    {
        public MerchantInfoViewModel MerchantInfo { get; set; }
        public string InvoiceName { get; set; }
        public string TransactionReference { get; set; }
        public long Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsDeleted { get; set; }
        public bool TransactionStatus { get; set; }
        public decimal ShippingFee { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class InvoicePaymentInfoViewModel
    {
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Channel { get; set; }
        public string TransactionStatus { get; set; }
        public string Message { get; set; }
        public long Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Status { get; set; }
        public DateTime DateEntered { get; set; } 
    }

    public class UserJourneyViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public DateTime DateEntered { get; set; }
        public DateTime LastDateModified { get; set; }

    }
}
