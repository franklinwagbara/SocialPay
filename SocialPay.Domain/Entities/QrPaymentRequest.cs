using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column(TypeName = "NVARCHAR(90)")]
        public string OrderNo { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string OrderType { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string MchNo { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string SubMchNo { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string PaymentRequestReference { get; set; }
        public double Amount { get; set; }

        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; } 
        public ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<QrPaymentResponse> QrPaymentResponse { get; set; }

    }
}
