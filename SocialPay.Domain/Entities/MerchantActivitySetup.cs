using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class MerchantActivitySetup
    {
        public long MerchantActivitySetupId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string PayOrchargeMe { get; set; }
        public bool ReceiveEmail { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal WithinLagos { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal OutSideLagos { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal OutSideNigeria { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
