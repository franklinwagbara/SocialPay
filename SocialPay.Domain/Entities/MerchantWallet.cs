using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class MerchantWallet
    {
        public long MerchantWalletId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Mobile { get; set; }
        public string DoB { get; set; }
        public string Gender { get; set; }
        public string CurrencyCode { get; set; }
        public bool status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
