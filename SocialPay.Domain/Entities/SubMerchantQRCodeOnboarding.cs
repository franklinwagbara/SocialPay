using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class SubMerchantQRCodeOnboarding : BaseEntity
    {
        public SubMerchantQRCodeOnboarding()
        {
            SubMerchantQRCodeOnboardingResponse = new HashSet<SubMerchantQRCodeOnboardingResponse>();           
        }
        public long SubMerchantQRCodeOnboardingId { get; set; }
        public long MerchantQRCodeOnboardingId { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string MchNo { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string MerchantName { get; set; }
        [Column(TypeName = "NVARCHAR(80)")]
        public string MerchantEmail { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string MerchantPhoneNumber { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string SubFixed { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string SubAmount { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public MerchantQRCodeOnboarding MerchantQRCodeOnboarding { get; set; }
        public virtual ICollection<SubMerchantQRCodeOnboardingResponse> SubMerchantQRCodeOnboardingResponse { get; set; }

    }
}
