using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Domain.Entities
{
    public class CustomerOtherPaymentsInfo
    {
        public long CustomerOtherPaymentsInfoId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long CustomerId { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string TransactionReference { get; set; }
        public long MerchantPaymentSetupId { get; set; }
        [Column(TypeName = "VARCHAR(130)")]
        public string CustomerDescription { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "VARCHAR(30)")]
        public string Email { get; set; }
        [Column(TypeName = "VARCHAR(60)")]
        public string Fullname { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string PhoneNumber { get; set; }
        [Column(TypeName = "VARCHAR(50)")]
        public string Document { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string Channel { get; set; }
        [Column(TypeName = "VARCHAR(130)")]
        public string FileLocation { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string PaymentReference { get; set; }
        public bool PaymentStatus { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual MerchantPaymentSetup MerchantPaymentSetup { get; set; }
    }
}
