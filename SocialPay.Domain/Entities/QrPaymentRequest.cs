using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class QrPaymentRequest : BaseEntity
    {
        public QrPaymentRequest()
        {
            QrPaymentResponse = new HashSet<QrPaymentResponse>();
        }
        public long QrPaymentRequestId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string OrderNo { get; set; }
        public string OrderType { get; set; }
        public string MchNo { get; set; }
        public string SubMchNo { get; set; }
        public string PaymentRequestReference { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; } 
        public ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<QrPaymentResponse> QrPaymentResponse { get; set; }

    }
}
