using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SendOtpRequest
    {
        public long SendOtpRequestId { get; set; }
        public int otp { get; set; }
        public string cardId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
