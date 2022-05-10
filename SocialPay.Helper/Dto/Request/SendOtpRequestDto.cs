using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class SendOtpRequestDto
    {
        public int otp { get; set; }
        public string cardId { get; set; }
        public string Email { get; set; }
    }
}
