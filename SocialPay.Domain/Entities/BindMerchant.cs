using System;
using System.Collections.Generic;

namespace SocialPay.Domain.Entities
{
    public class BindMerchant : BaseEntity
    {
        public BindMerchant()
        {
            BindMerchantResponse = new HashSet<BindMerchantResponse>();
        }
        public long BindMerchantId { get; set; }
        public long MerchantQRCodeOnboardingId { get; set; }
        public string MchNo { get; set; }
        public string BankNo { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public MerchantQRCodeOnboarding MerchantQRCodeOnboarding { get; set; }
        public virtual ICollection<BindMerchantResponse> BindMerchantResponse { get; set; }
    }
}
