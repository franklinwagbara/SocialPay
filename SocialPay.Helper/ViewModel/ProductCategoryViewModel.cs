using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class ProductCategoryViewModel
    {
        public long ProductCategoryId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
    }
}
