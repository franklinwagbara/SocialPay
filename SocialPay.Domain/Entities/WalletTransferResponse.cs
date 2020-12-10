using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class WalletTransferResponse
    {
        public long WalletTransferResponseId { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string RequestId { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string PaymentReference { get; set; }
        [Column(TypeName = "VARCHAR(120)")]
        public string message { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string response { get; set; }
        [Column(TypeName = "VARCHAR(150)")]
        public string responsedata { get; set; }
        public bool sent { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
       // public virtual WalletTransferRequestLog WalletTransferRequestLog { get; set; }
    }
}
