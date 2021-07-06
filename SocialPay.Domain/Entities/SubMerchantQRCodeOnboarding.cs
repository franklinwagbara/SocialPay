using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class SubMerchantQRCodeOnboarding : BaseEntity
    {
        public SubMerchantQRCodeOnboarding()
        {
            MerchantQRCodeOnboardingResponse = new HashSet<MerchantQRCodeOnboardingResponse>();           
        }
        public long SubMerchantQRCodeOnboardingId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string Name { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string Tin { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string Contact { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string Phone { get; set; }
        [Column(TypeName = "NVARCHAR(60)")]
        public string Email { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string Address { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<MerchantQRCodeOnboardingResponse> MerchantQRCodeOnboardingResponse { get; set; }

    }
}
