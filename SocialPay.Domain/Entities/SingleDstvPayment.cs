using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SingleDstvPayment : BaseEntity
    {
        public SingleDstvPayment()
        {
            SingleDstvPaymentResponse = new HashSet<SingleDstvPaymentResponse>();
        }
        public long SingleDstvPaymentId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string AccountLookupReference { get; set; }
        public string amountInCents { get; set; }
        public string merchantId { get; set; }
        public string merchantReference { get; set; }
        public string transactionType { get; set; }
        public string vasId { get; set; }
        public string countryCode { get; set; }
        public string customerId { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<SingleDstvPaymentResponse> SingleDstvPaymentResponse { get; set; }
    }

}
