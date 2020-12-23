using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialPay.Domain.Entities
{
    public class FailedTransactions
    {
        public long FailedTransactionsId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string CustomerTransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(550)")]
        public string Message { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
