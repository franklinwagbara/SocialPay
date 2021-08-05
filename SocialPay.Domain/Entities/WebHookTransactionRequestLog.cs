using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class WebHookTransactionRequestLog : BaseEntity
    {
        public long WebHookTransactionRequestLogId { get; set; }
        public string NotificationType { get; set; }
        public string TimeStamp { get; set; }
        public string MerchantName { get; set; }
        public string MerchantNo { get; set; }
        public string SubMerchantName { get; set; }
        public string SubMerchantNo { get; set; }
        public string TransactionTime { get; set; }
        public string TransactionAmount { get; set; }
        public string MerchantFee { get; set; }
        public string ResidualAmount { get; set; }
        public string TransactionType { get; set; }
        public string OrderSn { get; set; }
        public string OrderNo { get; set; }
        public string Sign { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
