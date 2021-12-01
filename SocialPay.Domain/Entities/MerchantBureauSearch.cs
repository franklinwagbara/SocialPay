using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class MerchantBureauSearch : BaseEntity
    {
        public long MerchantBureauSearchId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public bool isCustomerClean { get; set; }
        public string response { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
