using System;
using System.Collections.Generic;
using System.Text;

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
