using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ChargeCardRequest
    {
        public long ChargeCardRequestId { get; set; }
        public string pan { get; set; }
        public string cvv { get; set; }
        public int expiryMonth { get; set; }
        public int expiryYear { get; set; }
        public int currency { get; set; }
        public string pin { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
