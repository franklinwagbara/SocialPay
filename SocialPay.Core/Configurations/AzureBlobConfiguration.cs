using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Core.Configurations
{
    public class AzureBlobConfiguration
    {
        public string blobConnectionstring { get; set; }
        public string containerName { get; set; }
    }
}
