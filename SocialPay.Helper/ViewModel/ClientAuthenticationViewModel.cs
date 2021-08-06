using System;

namespace SocialPay.Helper.ViewModel
{
    public class ClientAuthenticationViewModel
    {
        public long ClientAuthenticationId { get; set; }
        public string Bvn { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }    
        public string StatusCode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        public string RoleName { get; set; }
        public string ReferralCode { get; set; }
        public string ReferCode { get; set; }
        public bool HasRegisteredCompany { get; set; }
        public string QrCodeStatus { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
    }
}
