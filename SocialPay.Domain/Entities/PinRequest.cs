using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class PinRequest
    {
        public long PinRequestId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string Pin { get; set; }
        public string TokenSecret { get; set; }
        public bool Status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
