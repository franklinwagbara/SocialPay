using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class FioranoBillsPaymentResponse : BaseEntity
    {
        public long FioranoBillsPaymentResponseId { get; set; }
        public long FioranoBillsRequestId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string PaymentReference { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string ReferenceID { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string ResponseCode { get; set; }
        [Column(TypeName = "NVARCHAR(280)")]
        public string ResponseText { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string Balance { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string COMMAMT { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string CHARGEAMT { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string FTID { get; set; }
        [Column(TypeName = "NVARCHAR(550)")]
        public string JsonResponse { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public virtual FioranoBillsRequest FioranoBillsRequest { get; set; }
    }
}
