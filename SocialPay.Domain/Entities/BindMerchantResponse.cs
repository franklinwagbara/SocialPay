using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class BindMerchantResponse : BaseEntity
    {
        public long BindMerchantResponseId { get; set; }
        public long BindMerchantId { get; set; }
        public string ReturnCode { get; set; }
        public string Mch_no { get; set; }
        public string JsonResponse { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public BindMerchant BindMerchant { get; set; }
    }
}
