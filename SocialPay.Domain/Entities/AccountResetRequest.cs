using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class AccountResetRequest
    {
        public long AccountResetRequestId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string Token { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
