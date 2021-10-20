using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class RequestTicketDto
    {
        public string accountNumber { get; set; }
        public string bankId { get; set; }
        public int customerId { get; set; }
    }
}
