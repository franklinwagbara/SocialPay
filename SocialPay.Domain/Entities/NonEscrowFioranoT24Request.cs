using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class NonEscrowFioranoT24Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "NVARCHAR(90)")]
        public string PaymentReference { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string TransactionBranch { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string TransactionType { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string DebitAcctNo { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string DebitCurrency { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string CreditCurrency { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string CreditAccountNo { get; set; }
        [Column(TypeName = "NVARCHAR(15)")]
        public string CommissionCode { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string VtellerAppID { get; set; }
        [Column(TypeName = "NVARCHAR(530)")]
        public string narrations { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string SessionId { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string TrxnLocation { get; set; }
        [Column(TypeName = "NVARCHAR(980)")]
        public string JsonRequest { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string Channel { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string Message { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        // public virtual ICollection<FioranoT24TransactionResponse> FioranoT24TransactionResponse { get; set; }
    }

}
