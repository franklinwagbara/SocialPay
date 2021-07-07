using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialPay.Domain.Entities
{
    public class MerchantPaymentSetup
    {
        public long MerchantPaymentSetupId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string PaymentLinkName { get; set; }
        [Column(TypeName = "NVARCHAR(120)")]
        public string MerchantDescription { get; set; }
        [Column(TypeName = "NVARCHAR(120)")]
        public string CustomerDescription { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MerchantAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CustomerAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AdditionalCharges { get; set; }
        public bool HasAdditionalCharges { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string CustomUrl { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string DeliveryMethod { get; set; }
        public long DeliveryTime { get; set; }
        public bool RedirectAfterPayment { get; set; }
        [Column(TypeName = "NVARCHAR(120)")]
        public string AdditionalDetails { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string PaymentCategory { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string PaymentMethod { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string PaymentLinkUrl { get; set; }
        [Column(TypeName = "NVARCHAR(120)")]
        public string Document { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string FileLocation { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<CustomerTransaction> CustomerTransaction { get; set; }
        public virtual ICollection<CustomerOtherPaymentsInfo> CustomerOtherPaymentsInfo { get; set; }
    }
}
