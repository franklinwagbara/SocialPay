using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class UsedPaymentChannel
    {
        public string paymentChannel { get; set; }
        public decimal Amount { get; set; }
    }
}
