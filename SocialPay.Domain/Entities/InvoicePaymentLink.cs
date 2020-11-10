using System;
using System.Collections.Generic;

namespace SocialPay.Domain.Entities
{
    public class InvoicePaymentLink
    {
        public long InvoicePaymentLinkId { get; set; }
        public long ClientAuthenticationId { get; set; }
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
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<InvoicePaymentInfo> InvoicePaymentInfo { get; set; }
    }
}
