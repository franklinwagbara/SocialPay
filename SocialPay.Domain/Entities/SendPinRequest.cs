using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SendPinRequest
    {
        public long SendPinRequestId { get; set; }
        public string pin { get; set; }
        public string cardId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
