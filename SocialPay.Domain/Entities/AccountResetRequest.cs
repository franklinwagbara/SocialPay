using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class AccountResetRequest
    {
        public long AccountResetRequestId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string Token { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
