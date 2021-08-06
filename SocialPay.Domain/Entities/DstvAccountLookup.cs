using System;
using System.Collections.Generic;

namespace SocialPay.Domain.Entities
{
    public class DstvAccountLookup : BaseEntity
    {
        public DstvAccountLookup()
        {
            DstvAccountLookupResponse = new HashSet<DstvAccountLookupResponse>();
        }
        public long DstvAccountLookupId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string merchantReference { get; set; }
        public string transactionType { get; set; }
        public string vasId { get; set; }
        public string countryCode { get; set; }
        public string customerId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<DstvAccountLookupResponse> DstvAccountLookupResponse { get; set; }

    }
}
