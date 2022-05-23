using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SendOtpResponse
    {
        public long SendOtpResponseId { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public string displayText { get; set; }
        public string status { get; set; }
        public string authUrl { get; set; }
        public string cardStatus { get; set; }
        public string bankName { get; set; }
        public string cardId { get; set; }
        public bool isValidCardForStatement { get; set; }
        public bool isSterling { get; set; }
        public bool success { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
