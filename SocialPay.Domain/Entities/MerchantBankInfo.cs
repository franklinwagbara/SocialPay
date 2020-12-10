using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class MerchantBankInfo
    {
        public long MerchantBankInfoId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "VARCHAR(30)")]
        public string BankName { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string BankCode { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string BranchCode { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string LedCode { get; set; }
        [Column(TypeName = "VARCHAR(15)")]
        public string Nuban { get; set; }
        [Column(TypeName = "VARCHAR(35)")]
        public string AccountName { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string Currency { get; set; }
        [Column(TypeName = "VARCHAR(12)")]
        public string BVN { get; set; }
        [Column(TypeName = "VARCHAR(25)")]
        public string Country { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string CusNum { get; set; }
        [Column(TypeName = "VARCHAR(5)")]
        public string KycLevel { get; set; }
        public bool DefaultAccount { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
