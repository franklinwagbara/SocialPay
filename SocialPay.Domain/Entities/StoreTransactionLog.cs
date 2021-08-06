using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class StoreTransactionLog : BaseEntity
    {
        public long StoreTransactionLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string Color { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string Size { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
