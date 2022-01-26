using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class MerchantsWithOutPaymentLinkResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Bvn { get; set; }
        public string ReferCode { get; set; }
        public string ReferralCode { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime LastDateModified { get; set; }
    }
}
