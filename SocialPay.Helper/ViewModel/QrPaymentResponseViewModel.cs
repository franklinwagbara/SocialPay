using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class QrPaymentResponseViewModel
    {
        public long QrPaymentResponseId { get; set; }
        public long QrPaymentRequestId { get; set; }
        public string OrderSn { get; set; }
        public string CodeUrl { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMsg { get; set; }
        public string PaymentReference { get; set; }
        public double Amount { get; set; }
        public string SessionID { get; set; }
        public DateTime DateEntered { get; set; } 
        public DateTime LastDateModified { get; set; }
    }
}
