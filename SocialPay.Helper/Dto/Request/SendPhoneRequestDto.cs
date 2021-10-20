using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class SendPhoneRequestDto
    {
        public string phoneNumber { get; set; }
        public string cardId { get; set; }
    }
}
