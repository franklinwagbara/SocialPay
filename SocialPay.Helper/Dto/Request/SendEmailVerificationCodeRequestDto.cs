using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class SendEmailVerificationCodeRequestDto
    {
        public string clientBaseUrl { get; set; }
        public string email { get; set; }
        public string verificationCodeParameterName { get; set; }
        public string emailParameterName { get; set; }
    }
}
