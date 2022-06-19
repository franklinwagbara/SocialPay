using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SendPhoneRequest
    {
        public long SendPhoneRequestId { get; set; }
        public string phoneNumber { get; set; }
        public string cardId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
