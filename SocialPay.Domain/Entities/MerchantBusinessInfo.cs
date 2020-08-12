using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class MerchantBusinessInfo
    {
        public long MerchantBusinessInfoId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string MerchantReferenceId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessPhoneNumber { get; set; }
        public string BusinessEmail { get; set; }
        public string Country { get; set; }
        public string Chargebackemail { get; set; }
        public string Logo { get; set; }
        public string FileLocation { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
