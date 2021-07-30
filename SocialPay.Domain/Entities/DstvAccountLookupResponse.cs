using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class DstvAccountLookupResponse : BaseEntity
    {
        public long DstvAccountLookupResponseId { get; set; }
        public long DstvAccountLookupId { get; set; }
        public string merchantReference { get; set; }
        public string payUVasReference { get; set; }
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
        public string pointOfFailure { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual DstvAccountLookup DstvAccountLookup { get; set; }
    }
}
