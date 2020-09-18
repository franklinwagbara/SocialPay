using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class CustomerResponseDto
    {
        public long CustomerId { get; set; }
        public string PaymentLink { get; set; }
    }
}
