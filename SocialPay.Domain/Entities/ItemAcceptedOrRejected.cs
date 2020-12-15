using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class ItemAcceptedOrRejected
    {
        public long ItemAcceptedOrRejectedId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long CustomerTransactionId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string CustomerTransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string PaymentReference { get; set; }
        [Column(TypeName = "NVARCHAR(180)")]
        public string Comment { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string Status { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string ProcessedBy { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string OrderStatus { get; set; }
        public bool IsReturned { get; set; }
        public DateTime ReturnedDate { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
