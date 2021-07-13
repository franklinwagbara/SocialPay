using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class DynamicPaymentRequestDto
    {
        public string amount { get; set; }       
    }

    public class DynamicPaymentDefaultRequestDto : DynamicPaymentRequestDto
    {
        public string orderNo { get; set; }
        public string orderType { get; set; }     
        public string mchNo { get; set; }
        public string subMchNo { get; set; }
    }
}
