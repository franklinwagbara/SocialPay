using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ConfirmTicketRequest : BaseEntity
    {
        public long ConfirmTicketRequestId { get; set; }
        public string ticketNumber { get; set; }
        public string ticketPassword { get; set; }
        public int customerId { get; set; }
        public string bankId { get; set; }
        public string accountNumber { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
