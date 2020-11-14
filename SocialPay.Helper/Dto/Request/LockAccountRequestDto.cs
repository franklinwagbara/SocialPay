using System;

namespace SocialPay.Helper.Dto.Request
{
    public class LockAccountRequestDto
    {
        public string acct { get; set; }
        public DateTime sDate { get; set; }
        public DateTime eDate { get; set; }
        public decimal amt { get; set; }
        public string reasonForLocking { get; set; }
    }
}
