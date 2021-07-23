using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class StoreViewModel
    {
        public long MerchantStoreId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string StoreName { get; set; }
        public string StoreLink { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
        public string FileLocation { get; set; }
        public DateTime DateEntered { get; set; }
    }
}
