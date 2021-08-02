using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class MerchantQRCodeOnboarding : BaseEntity
    {
        public MerchantQRCodeOnboarding()
        {
            MerchantQRCodeOnboardingResponse = new HashSet<MerchantQRCodeOnboardingResponse>();
            BindMerchant = new HashSet<BindMerchant>();           
        }
        public long MerchantQRCodeOnboardingId { get; set; }
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
        public bool IsCompleted { get; set; }
        public bool IsDeleted { get; set; }
        public string Status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<MerchantQRCodeOnboardingResponse> MerchantQRCodeOnboardingResponse { get; set; }
        public virtual ICollection<SubMerchantQRCodeOnboarding> SubMerchantQRCodeOnboarding { get; set; }
        public virtual ICollection<BindMerchant> BindMerchant { get; set; }

    }
}
