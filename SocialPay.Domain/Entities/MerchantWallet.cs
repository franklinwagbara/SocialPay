using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class MerchantWallet
    {
        public long MerchantWalletId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string Firstname { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string Lastname { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string Mobile { get; set; }
        [Column(TypeName = "VARCHAR(15)")]
        public string DoB { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string Gender { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string CurrencyCode { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
