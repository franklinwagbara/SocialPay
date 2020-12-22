using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class CustomerOtherPaymentsInfo
    {
        public long CustomerOtherPaymentsInfoId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long CustomerId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        public long MerchantPaymentSetupId { get; set; }
        [Column(TypeName = "NVARCHAR(130)")]
        public string CustomerDescription { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string Email { get; set; }
        [Column(TypeName = "NVARCHAR(60)")]
        public string Fullname { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string PhoneNumber { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string Document { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string Channel { get; set; }
        [Column(TypeName = "NVARCHAR(130)")]
        public string FileLocation { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string PaymentReference { get; set; }
        public bool PaymentStatus { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual MerchantPaymentSetup MerchantPaymentSetup { get; set; }
    }
}
