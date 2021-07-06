using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class PersonalInfoViewModel
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
        public DateTime DateEntered { get; set; } 
    }
}
