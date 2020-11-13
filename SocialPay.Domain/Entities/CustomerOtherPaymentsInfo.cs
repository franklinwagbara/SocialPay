using System;

namespace SocialPay.Domain.Entities
{
    public class CustomerOtherPaymentsInfo
    {
        public long CustomerOtherPaymentsInfoId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long CustomerId { get; set; }
        public long MerchantPaymentSetupId { get; set; }
        public string CustomerDescription { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Document { get; set; }
        public string Channel { get; set; }
        public string FileLocation { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual MerchantPaymentSetup MerchantPaymentSetup { get; set; }
    }
}
