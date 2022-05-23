using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ValidateChargeRequest
    {
        public long ValidateChargeRequestId { get; set; }
        public string cardId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;

    }
}
