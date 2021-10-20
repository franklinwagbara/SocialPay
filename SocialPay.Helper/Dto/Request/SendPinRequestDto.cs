using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class SendPinRequestDto
    {
        public string pin { get; set; }
        public string cardId { get; set; }
    }
}
