using System;

namespace SocialPay.Domain.Entities
{
    public class WalletTransferResponse
    {
        public long WalletTransferResponseId { get; set; }
        public long WalletTransferRequestLogId { get; set; }
        public string message { get; set; }
        public string response { get; set; }
        public string responsedata { get; set; }
        public bool sent { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public virtual WalletTransferRequestLog WalletTransferRequestLog { get; set; }
    }
}
