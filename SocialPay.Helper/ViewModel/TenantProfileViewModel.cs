using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class TenantProfileViewModel
    {
        public long TenantProfileId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string TenantName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string WebSiteUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthKey { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; }
        public DateTime LastDateModified { get; set; }
    }
}
