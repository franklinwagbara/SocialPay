using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Configurations
{
    public class SerilogConfiguration
    {
        public string storelogger { get; set; }
        public string merchantAdvancelogger { get; set; }
        public string containerName { get; set; }
        public string blobBaseUrl { get; set; }
    }
}
