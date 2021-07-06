using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class MerchantQRCodeOnboardingResponse : BaseEntity
    {
        public long MerchantQRCodeOnboardingResponseId { get; set; }
        public long MerchantQRCodeOnboardingId { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string ReturnCode { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string ReturnMsg { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string MchNo { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string MerchantName { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string MerchantTIN { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string MerchantAddress { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string MerchantContactName { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string MerchantPhoneNumber { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string MerchantEmail { get; set; }
        public string JsonResponse { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public MerchantQRCodeOnboarding MerchantQRCodeOnboarding { get; set; }
    }
}
