using System;

namespace SocialPay.Domain.Entities
{
    public class MerchantActivitySetup
    {
        public long MerchantActivitySetupId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string PayOrchargeMe { get; set; }
        public bool ReceiveEmail { get; set; }
        public decimal WithinLagos { get; set; }
        public decimal OutSideLagos { get; set; }
        public decimal OutSideNigeria { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
