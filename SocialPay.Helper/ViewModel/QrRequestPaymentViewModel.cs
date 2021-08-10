using System;

namespace SocialPay.Helper.ViewModel
{
    public class QrRequestPaymentViewModel
    {
        public long QrPaymentRequestId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string OrderNo { get; set; }
        public double Amount { get; set; }
        public string OrderType { get; set; }
        public string MchNo { get; set; }
        public string SubMchNo { get; set; }
        public string PaymentRequestReference { get; set; }
        public DateTime DateEntered { get; set; } 
        public DateTime LastDateModified { get; set; }
    }
}
