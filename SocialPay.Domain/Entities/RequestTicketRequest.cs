using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class RequestTicketRequest : BaseEntity
    {
        public long RequestTicketRequestId { get; set; }
        public string accountNumber { get; set; }
        public string bankId { get; set; }
        public int customerId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
