using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ClientLoginStatus
    {
        public long ClientLoginStatusId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public bool IsSuccessful { get; set; }
        public int LoginAttempt { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
