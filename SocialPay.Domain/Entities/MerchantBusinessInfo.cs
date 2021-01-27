using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialPay.Domain.Entities
{
    public class MerchantBusinessInfo
    {
        public long MerchantBusinessInfoId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string MerchantReferenceId { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string SpectaMerchantID { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string SpectaMerchantKey { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string SpectaMerchantKeyValue { get; set; }
        public bool HasSpectaMerchantID { get; set; }
        [Column(TypeName = "NVARCHAR(65)")]
        public string BusinessName { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string BusinessPhoneNumber { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string BusinessEmail { get; set; }
        [Column(TypeName = "NVARCHAR(25)")]
        public string Country { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string Tin { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string Chargebackemail { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string Logo { get; set; }
        [Column(TypeName = "NVARCHAR(190)")]
        public string FileLocation { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
