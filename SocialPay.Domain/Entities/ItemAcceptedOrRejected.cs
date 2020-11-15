﻿using System;

namespace SocialPay.Domain.Entities
{
    public class ItemAcceptedOrRejected
    {
        public long ItemAcceptedOrRejectedId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long CustomerTransactionId { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string PaymentReference { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public string ProcessedBy { get; set; }
        public string OrderStatus { get; set; }
        public bool IsReturned { get; set; }
        public DateTime ReturnedDate { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
