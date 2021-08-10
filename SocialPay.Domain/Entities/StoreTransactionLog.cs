using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class StoreTransactionLog : BaseEntity
    {
        public StoreTransactionLog()
        {
            StoreTransactionLogDetails = new HashSet<StoreTransactionLogDetails>();
        }
        public long StoreTransactionLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string PaymentReference { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string TransactionStatus { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<StoreTransactionLogDetails> StoreTransactionLogDetails { get; set; }

    }
}
