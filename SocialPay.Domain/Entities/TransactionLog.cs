﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class TransactionLog
    {
        public long TransactionLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string CustomerEmail { get; set; }
        public string Category { get; set; }
        public string OrderStatus { get; set; }
        public string Message { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public bool Status { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}