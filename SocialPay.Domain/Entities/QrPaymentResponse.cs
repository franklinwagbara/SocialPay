﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class QrPaymentResponse : BaseEntity
    {
        public long QrPaymentResponseId { get; set; }
        public long QrPaymentRequestId { get; set; }
        [Column(TypeName = "NVARCHAR(190)")]
        public string OrderSn { get; set; }
        public string CodeUrl { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string ReturnCode { get; set; }
        public string ReturnMsg { get; set; }
        public string PaymentReference { get; set; }
        public double Amount { get; set; }
        public string SessionID { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public QrPaymentRequest QrPaymentRequest { get; set; }
    }
}
