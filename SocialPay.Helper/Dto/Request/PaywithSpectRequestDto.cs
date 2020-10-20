using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class PaywithSpectRequestDto
    {
        public string callBackUrl { get; set; }
        public string reference { get; set; }
        public string merchantId { get; set; }
        public string description { get; set; }
        public string amount { get; set; }
    }
}
