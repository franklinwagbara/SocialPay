﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class OtherMerchantBankInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MerchantOtherBankInfoId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string BankName { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string BankCode { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string BranchCode { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string LedCode { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string Nuban { get; set; }
        [Column(TypeName = "NVARCHAR(65)")]
        public string AccountName { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string Currency { get; set; }
        [Column(TypeName = "NVARCHAR(12)")]
        public string BVN { get; set; }
        [Column(TypeName = "NVARCHAR(25)")]
        public string Country { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string CusNum { get; set; }
        [Column(TypeName = "NVARCHAR(5)")]
        public string KycLevel { get; set; }
        public bool DefaultAccount { get; set; }
        public bool Deleted { get; set; } = false;
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }

    }

}
