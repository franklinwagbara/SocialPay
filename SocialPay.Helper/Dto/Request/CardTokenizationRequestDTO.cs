using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{

    public class CardTokenizationRequestDTO
    {
        public string fullName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string dob { get; set; }
        public string tokenType { get; set; }
        public string channel { get; set; }
        public string cardMinExpiryInMonths { get; set; }
        public string redirectUrl { get; set; }
        public string bvn { get; set; }
        public float amount { get; set; }

    }

    public class CardTokenizationAuthorizationRequestDTO
    {
        public string AuthorizationToken { get; set; }
    }

    public class DebitTokenizedCard
    {
        public string authorization_code { get; set; }
        public string email { get; set; }
        public string amount { get; set; }
        public string reference { get; set; }
    }

}
