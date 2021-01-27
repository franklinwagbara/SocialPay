using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class GuestAccountLog
    {
        public long GuestAccountLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
