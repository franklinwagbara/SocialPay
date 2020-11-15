using System;

namespace SocialPay.Domain.Entities
{
    public class InvoicePaymentInfo
    {
        public long InvoicePaymentInfoId { get; set; }
        public long InvoicePaymentLinkId { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Channel { get; set; }
        public string TransactionStatus { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual InvoicePaymentLink InvoicePaymentLink { get; set; }
    }
}
