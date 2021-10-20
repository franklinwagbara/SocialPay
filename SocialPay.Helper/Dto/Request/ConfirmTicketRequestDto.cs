using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class ConfirmTicketRequestDto
    {
        public string ticketNumber { get; set; }
        public string ticketPassword { get; set; }
        public int customerId { get; set; }
        public string bankId { get; set; }
        public string accountNumber { get; set; }
    }
}
