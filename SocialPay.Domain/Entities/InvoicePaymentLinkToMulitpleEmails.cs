using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class InvoicePaymentLinkToMulitpleEmails
    {
        public long InvoicePaymentLinkToMulitpleEmailsId { get; set; }

        public long InvoicePaymentLinkId { get; set; }

        public string email { get; set; }
        public string status { get; set; }

        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual InvoicePaymentLink InvoicePaymentLink { get; set; }
    }
}
