using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class VendAirtimeRequestLog : BaseEntity
    {
        public long VendAirtimeRequestLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(25)")]
        public string ReferenceId { get; set; }
        [Column(TypeName = "NVARCHAR(55)")]
        public string Translocation { get; set; }
        [Column(TypeName = "NVARCHAR(55)")]
        public string email { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string SubscriberInfo1 { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string nuban { get; set; }
        [Column(TypeName = "NVARCHAR(45)")]
        public string TransactionType { get; set; }
        public int AppId { get; set; }
        public int RequestType { get; set; }
        public string TerminalID { get; set; }
        [Column(TypeName = "NVARCHAR(45)")]
        public string Paymentcode { get; set; }
        public string Mobile { get; set; }
        public double Amount { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
