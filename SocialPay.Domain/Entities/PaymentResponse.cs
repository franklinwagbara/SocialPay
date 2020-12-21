﻿using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialPay.Domain.Entities
{
    public class PaymentResponse
    {
        public long PaymentResponseId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string PaymentReference { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string Message { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }
}
