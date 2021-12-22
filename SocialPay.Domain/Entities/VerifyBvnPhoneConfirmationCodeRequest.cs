using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class VerifyBvnPhoneConfirmationCodeRequest : BaseEntity
    {
        public long VerifyBvnPhoneConfirmationCodeRequestId { get; set; }
        public string token { get; set; }
        public string email { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
