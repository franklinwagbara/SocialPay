using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class MerchantTransactionSetupViewModel
    {
        public long MerchantTransactionSetupId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string Pin { get; set; }
        public bool Status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
    }
}
