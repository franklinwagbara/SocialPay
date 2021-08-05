using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class WebHookRequest : BaseEntity
    {
        public long WebHookRequestId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string description { get; set; }
        public long webHookUri { get; set; }
        public bool isActive { get; set; }
        public string filters { get; set; }
        public string headers { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
