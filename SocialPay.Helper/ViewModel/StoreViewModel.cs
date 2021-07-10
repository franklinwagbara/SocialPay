using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class StoreViewModel
    {
        public long StoreId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string StoreName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string FileLocation { get; set; }
        public DateTime DateEntered { get; set; }
    }
}
