using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ItemAcceptedOrRejected
    {
        public long ItemAcceptedOrRejectedId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long CustomerTransactionId { get; set; }
        public string TransactionReference { get; set; }
        public string Comment { get; set; }
        public bool Status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
