using System;

namespace SocialPay.Domain.Entities
{
    public class LoginAttemptHistory
    {
        public long LoginAttemptHistoryId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
