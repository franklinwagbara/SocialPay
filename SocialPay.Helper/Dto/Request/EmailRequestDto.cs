using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class EmailRequestDto
    {
        public string SourceEmail { get; set; }
        public string DestinationEmail { get; set; }
        public string Subject { get; set; }
        public string EmailBody { get; set; }
    }
}
