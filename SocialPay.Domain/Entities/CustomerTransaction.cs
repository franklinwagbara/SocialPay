using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class CustomerTransaction
    {
        public long CustomerTransactionId { get; set; }
        public long MerchantPaymentSetupId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "VARCHAR(30)")]
        public string CustomerEmail { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string Channel { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string OrderStatus { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string Message { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string CustomerTransactionReference { get; set; }
        public bool Status { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public virtual MerchantPaymentSetup MerchantPaymentSetup { get; set; }
    }
}
