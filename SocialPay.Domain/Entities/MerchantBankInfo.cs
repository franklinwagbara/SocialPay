using System;

namespace SocialPay.Domain.Entities
{
    public class MerchantBankInfo
    {
        public long MerchantBankInfoId { get; set; }
        public long ClientAuthenticationId { get; set; }        
        public string BankName { get; set; }
        public string Nuban { get; set; }
        public string AccountName { get; set; }
        public string Currency { get; set; }
        public string BVN { get; set; }
        public string Country { get; set; }
        public bool DefaultAccount { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
