using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class FailedTransactions
    {
        public long FailedTransactionsId { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string Message { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
