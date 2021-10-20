using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class VerifyEmailConfirmationCodeRequestDto
    {
        public string token { get; set; }
        public string email { get; set; }
    }
}
