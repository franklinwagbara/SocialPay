﻿using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialPay.Domain.Entities
{
    public class MerchantBusinessInfo
    {
        public long MerchantBusinessInfoId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string MerchantReferenceId { get; set; }
        [Column(TypeName = "VARCHAR(50)")]
        public string BusinessName { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string BusinessPhoneNumber { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string BusinessEmail { get; set; }
        [Column(TypeName = "VARCHAR(25)")]
        public string Country { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string Chargebackemail { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string Logo { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string FileLocation { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
