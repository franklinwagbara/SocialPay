using System;

namespace SocialPay.Domain.Entities
{
    public class DisputeRequestLog
    {
        public long DisputeRequestLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string DisputeComment { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string DisputeFile { get; set; }
        public string FileLocation { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
