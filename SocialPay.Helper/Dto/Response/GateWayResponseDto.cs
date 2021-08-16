using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class GateWayResponseDto
    {
        public string responseCode { get; set; }
        public string responsemessage { get; set; }
        public string reference { get; set; }
        public float amount { get; set; }
        public string terminalId { get; set; }
        public string merchantId { get; set; }
        public string retrievalReference { get; set; }
        public string institutionCode { get; set; }
        public string shortName { get; set; }
        public string customer_mobile { get; set; }
        public string SubMerchantName { get; set; }
        public string TransactionID { get; set; }
        public string UserID { get; set; }
        public string TraceID { get; set; }
    }
}
