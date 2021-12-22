using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SendEmailVerificationCodeRequest : BaseEntity
    {
        public long SendEmailVerificationCodeRequestId { get; set; }
        public string clientBaseUrl { get; set; }
        public string email { get; set; }
        public string verificationCodeParameterName { get; set; }
        public string emailParameterName { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
