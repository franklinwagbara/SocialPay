using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class DisputeRequestLog
    {
        public long DisputeRequestLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "VARCHAR(250)")]
        public string DisputeComment { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string PaymentReference { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string DisputeFile { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string ProcessedBy { get; set; }
        [Column(TypeName = "VARCHAR(130)")]
        public string FileLocation { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
