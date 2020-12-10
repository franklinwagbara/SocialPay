﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class WalletTransferRequestLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "VARCHAR(90)")]
        public string PaymentReference { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string RequestId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "VARCHAR(90)")]
        public string CustomerTransactionReference { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal amt { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string toacct { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string frmacct { get; set; }
        [Column(TypeName = "VARCHAR(130)")]
        public string remarks { get; set; }
        [Column(TypeName = "int")]
        public int channelID { get; set; }
        [Column(TypeName = "VARCHAR(10)")]
        public string CURRENCYCODE { get; set; }
        [Column(TypeName = "int")]
        public int TransferType { get; set; }
        [Column(TypeName = "VARCHAR(15)")]
        public string ChannelMode { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
