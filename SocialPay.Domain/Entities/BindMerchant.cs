using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class BindMerchant : BaseEntity
    {
        public long BindMerchantId { get; set; }
        public long MerchantQRCodeOnboardingId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public MerchantQRCodeOnboarding MerchantQRCodeOnboarding { get; set; }
    }
}
