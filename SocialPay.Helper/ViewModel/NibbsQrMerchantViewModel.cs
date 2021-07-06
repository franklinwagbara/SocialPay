using System;

namespace SocialPay.Helper.ViewModel
{
    public class NibbsQrMerchantViewModel
    {
        public long MerchantQRCodeOnboardingId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string Name { get; set; }
        public string Tin { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal Fee { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; } 
    }
}
