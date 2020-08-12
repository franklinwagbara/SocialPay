using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Core.Configurations
{
    public class AppSettings
    {
        public string appKey { get; set; }
        public string SecretKey { get; set; }
        public string EwsServiceUrl { get; set; }
        public string WebportalUrl { get; set; }
        public string BankServiceUrl { get; set; }
    }
}
