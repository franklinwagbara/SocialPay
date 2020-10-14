using System;
using System.Collections.Generic;

namespace SocialPay.Domain.Entities
{
    public class MerchantPaymentSetup
    {
        public long MerchantPaymentSetupId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string PaymentLinkName { get; set; }
        public string MerchantDescription { get; set; }
        public string CustomerDescription { get; set; }
        public decimal MerchantAmount { get; set; }
        public decimal CustomerAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AdditionalCharges { get; set; }
        public bool HasAdditionalCharges { get; set; }
        public string CustomUrl { get; set; }
        public string DeliveryMethod { get; set; }
        public long DeliveryTime { get; set; }
        public bool RedirectAfterPayment { get; set; }
        public string AdditionalDetails { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentLinkUrl { get; set; }
        public string Document { get; set; }
        public string FileLocation { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<CustomerTransaction> CustomerTransaction { get; set; }
        public virtual ICollection<CustomerOtherPaymentsInfo> CustomerOtherPaymentsInfo { get; set; }
    }
}
