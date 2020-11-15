using System;
using System.Collections.Generic;

namespace SocialPay.Domain.Entities
{
    public class FioranoT24Request
    {
        public long FioranoT24RequestId { get; set; }
        public string TransactionReference  { get; set; }
        public string TransactionBranch { get; set; }
        public string TransactionType { get; set; }
        public string DebitAcctNo { get; set; }
        public string DebitCurrency { get; set; }
        public string CreditCurrency { get; set; }
        public double DebitAmount { get; set; }
        public string CreditAccountNo { get; set; }
        public string CommissionCode { get; set; }
        public string VtellerAppID { get; set; }
        public string narrations { get; set; }
        public string SessionId { get; set; }
        public string TrxnLocation { get; set; }
        public string JsonRequest { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public virtual ICollection<FioranoT24TransactionResponse> FioranoT24TransactionResponse { get; set; }
    }

}
