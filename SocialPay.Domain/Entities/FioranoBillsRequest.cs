using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class FioranoBillsRequest : BaseEntity
    {
        public FioranoBillsRequest()
        {
            FioranoBillsPaymentResponse = new HashSet<FioranoBillsPaymentResponse>();
        }
        public long FioranoBillsRequestId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string TransactionReference { get; set; }
        public string TransactionBranch { get; set; }
        public string BillsType { get; set; }
        public string TransactionType { get; set; }
        public string DebitAcctNo { get; set; }
        public string DebitCurrency { get; set; }
        public string CreditCurrency { get; set; }
        public string DebitAmount { get; set; }
        public string CreditAccountNo { get; set; }
        public string CommissionCode { get; set; }
        public string VtellerAppID { get; set; }
        public string narrations { get; set; }
        public string SessionId { get; set; }
        public string TrxnLocation { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<FioranoBillsPaymentResponse> FioranoBillsPaymentResponse { get; set; }
    }
}
