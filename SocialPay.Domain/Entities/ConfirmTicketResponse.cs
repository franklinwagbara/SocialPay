using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ConfirmTicketResponse : BaseEntity
    {
        public int ConfirmTicketResponseId { get; set; }
        public string code { get; set; }
        public string shortDescription { get; set; }
        public double loanLimit { get; set; }
        public DateTime limitExpiryDate { get; set; }
        public bool success { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public string validationErrors { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
