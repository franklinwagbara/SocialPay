using System;

namespace SocialPay.Domain.Entities
{
    public class FundsTransferLog
    {
        public long FundsTransferLogId { get; set; }
        public string TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public bool IsDebited { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
    }
}
