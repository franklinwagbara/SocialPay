using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class SubMerchantQRCodeOnboardingResponse : BaseEntity
    {      
        public long SubMerchantQRCodeOnboardingResponseId { get; set; }
        public long SubMerchantQRCodeOnboardingId { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string ReturnCode { get; set; }
        [Column(TypeName = "NVARCHAR(130)")]
        public string ReturnMsg { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string MchNo { get; set; }
        [Column(TypeName = "NVARCHAR(130)")]
        public string MerchantName { get; set; }
        [Column(TypeName = "NVARCHAR(60)")]
        public string SubMchNo { get; set; }
        public string QrCode { get; set; }
        public string JsonResponse { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public SubMerchantQRCodeOnboarding SubMerchantQRCodeOnboarding { get; set; }
    }
}
