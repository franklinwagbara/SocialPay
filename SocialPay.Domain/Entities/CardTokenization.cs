using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class CardTokenization : BaseEntity
    {
        public long CardTokenizationId { get; set; }
        public long ClientAuthenticationId { get; set; }
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
        public string reference { get; set; }
        public string message { get; set; }
        public string responseUrl { get; set; }
        public bool status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }

}
