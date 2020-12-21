using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class InvoicePaymentLink
    {
        public long InvoicePaymentLinkId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string InvoiceName { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        public long Qty { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string CustomerEmail { get; set; }
        public bool IsDeleted { get; set; }
        public bool TransactionStatus { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<InvoicePaymentInfo> InvoicePaymentInfo { get; set; }
    }
}
