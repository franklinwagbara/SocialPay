using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class PaymentResponse
    {
        public long PaymentResponseId { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentReference { get; set; }
        public string Message { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }
}
