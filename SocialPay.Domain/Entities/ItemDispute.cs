using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ItemDispute
    {
        public long ItemDisputeId { get; set; }
        public long ItemAcceptedOrRejectedId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string DisputeComment { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ItemAcceptedOrRejected ItemAcceptedOrRejected { get; set; }
    }
}
