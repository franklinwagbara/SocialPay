using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class LoanDailyDeductionLog : BaseEntity
    {
        public long LoanDailyDeductionLogId { get; set; }
        public string TransactionId { get; set; }
        public long LoanDisbursementId { get; set; }
        public long ApplyForLoanId { get; set; }
        public decimal RepaymentAmount { get; set; }
        public decimal AmountDeducted { get; set; }
        public decimal OutstandingBalance { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string DebittNuban { get; set; }
        public bool status { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string TransactionBranch { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string TransactionType { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string DebitAcctNo { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string DebitCurrency { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string CreditCurrency { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string DebitAmount { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string CreditAccountNo { get; set; }
        [Column(TypeName = "NVARCHAR(80)")]
        public string CommissionCode { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string VtellerAppID { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string narrations { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string SessionId { get; set; }
        [Column(TypeName = "NVARCHAR(130)")]
        public string TrxnLocation { get; set; }
        [Column(TypeName = "NVARCHAR(130)")]
        public string message { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; } = DateTime.Now;
        public virtual ApplyForLoan ApplyForLoan { get; set; }
     //   public virtual LoanDisbursement LoanDisbursement { get; set; }

    }

}
