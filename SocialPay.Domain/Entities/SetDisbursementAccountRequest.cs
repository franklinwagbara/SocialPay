using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SetDisbursementAccountRequest : BaseEntity
    {
        public long SetDisbursementAccountRequestId { get; set; }
        public string disbAccountNumber { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
