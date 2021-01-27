using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialPay.Domain.Entities
{
    public class InvoicePaymentInfo
    {
        public long InvoicePaymentInfoId { get; set; }
        public long InvoicePaymentLinkId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string PaymentReference { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string CustomerTransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string Email { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string Fullname { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string PhoneNumber { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string Channel { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string TransactionStatus { get; set; }
        [Column(TypeName = "NVARCHAR(550)")]
        public string Message { get; set; }
        public bool Status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual InvoicePaymentLink InvoicePaymentLink { get; set; }
    }
}
