using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class UssdServiceRequestLog : BaseEntity
    {
       
        public long UssdServiceRequestLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string MerchantID { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string Channel { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string TransactionType { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransRef { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string MerchantName { get; set; }
        public double Amount { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TerminalId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string RetrievalReference { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string InstitutionCode { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string ShortName { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string Customer_mobile { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string SubMerchantName { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string UserID { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string ResponseCode { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string ResponseMessage { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string CallBackResponseCode { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string CallBackResponseMessage { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionRef { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionID { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TraceID { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; } 
        public virtual ClientAuthentication ClientAuthentication { get; set; }

    }
}
