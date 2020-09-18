using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class CustomerTransaction
    {
        public long CustomerTransactionId { get; set; }
        public long MerchantPaymentSetupId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string CustomerEmail { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public virtual MerchantPaymentSetup MerchantPaymentSetup { get; set; }
    }
}
