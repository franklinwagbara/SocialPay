using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Configurations
{
    public class SerilogConfiguration
    {
        
        public string storelogger { get; set; }
        public string merchantAdvancelogger { get; set; }
        public string escrowjoblogger { get; set; }
        public string interbankjoblogger { get; set; }
        public string fioranojoblogger { get; set; }
        public string nonescrowjoblogger { get; set; }
        public string notificationjoblogger { get; set; }
        public string walletjoblogger { get; set; }
        public string banktransactionjoblogger { get; set; }
        public string paywithcardjoblogger { get; set; }
        public string containerName { get; set; }
        public string blobBaseUrl { get; set; }
        public string accountLogger { get; set; }
        public string customerLogger { get; set; }
        public string fioranoT24Logger { get; set; }
        public string merchantsLogger { get; set; }
        public string transactionLogger { get; set; }
        public string spectaonboardinglogger { get; set; }
    }
}
