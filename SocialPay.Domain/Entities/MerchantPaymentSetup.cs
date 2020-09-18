using System;
using System.Collections.Generic;

namespace SocialPay.Domain.Entities
{
    public class MerchantPaymentSetup
    {
        public long MerchantPaymentSetupId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string PaymentLinkName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomUrl { get; set; }
        public string DeliveryMethod { get; set; }
        public string DeliveryTime { get; set; }
        public bool RedirectAfterPayment { get; set; }
        public string AdditionalDetails { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentLinkUrl { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<CustomerTransaction> CustomerTransaction { get; set; }
    }
}
