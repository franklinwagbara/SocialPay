using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SingleDstvPaymentResponse : BaseEntity
    {
        public long SingleDstvPaymentResponseId { get; set; }
        public long SingleDstvPaymentId { get; set; }
        public string merchantReference { get; set; }
        public string payUVasReference { get; set; }
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
        public string pointOfFailure { get; set; }
        public string merchantId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual SingleDstvPayment SingleDstvPayment { get; set; }
    }
}
