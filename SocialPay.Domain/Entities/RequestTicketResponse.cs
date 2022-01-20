using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class RequestTicketResponse : BaseEntity
    {
        public long RequestTicketResponseId { get; set; }
        public bool success { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public string validationErrors { get; set; }
        public string shortDescription { get; set; }
        public string requestId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }

}
