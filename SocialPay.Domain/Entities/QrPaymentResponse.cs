using System;

namespace SocialPay.Domain.Entities
{
    public class QrPaymentResponse
    {
        public long QrPaymentResponseId { get; set; }
        public long QrPaymentRequestId { get; set; }
        public string OrderSn { get; set; }
        public string CodeUrl { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMsg { get; set; }
        public string PaymentReference { get; set; }
        public string Amount { get; set; }
        public string SessionID { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public QrPaymentRequest QrPaymentRequest { get; set; }
    }
}
