﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class InterBankTransactionRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "NVARCHAR(90)")]
        public string PaymentReference { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string OriginatorKYCLevel { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string BeneficiaryKYCLevel { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string BeneficiaryBankVerificationNumber { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string OriginatorBankVerificationNumber { get; set; }
        [Column(TypeName = "int")]
        public int AppID { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string AccountLockID { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string OriginatorAccountNumber { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string AccountNumber { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string AccountName { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string DestinationBankCode { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string OrignatorName { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string SubAcctVal { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string LedCodeVal { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string CurCodeVal { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string CusNumVal { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string BraCodeVal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Vat { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "NVARCHAR(120)")]
        public string PaymentRef { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string NESessionID { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string ChannelCode { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
