using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class WebHookTransactionRequestLog : BaseEntity
    {
        public long WebHookTransactionRequestLogId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string NotificationType { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TimeStamp { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string MerchantName { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string MerchantNo { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string SubMerchantName { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string SubMerchantNo { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionTime { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string TransactionAmount { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string MerchantFee { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string ResidualAmount { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionType { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string OrderSn { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string OrderNo { get; set; }
        public string Sign { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
