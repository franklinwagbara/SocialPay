using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class LinkCategory
    {
        public long LinkCategoryId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string TransactionReference { get; set; }
        public string Channel { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
