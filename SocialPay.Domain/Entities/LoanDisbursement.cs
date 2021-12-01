using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class LoanDisbursement : BaseEntity
    {
        public long LoanDisbursementId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string TransactionReference { get; set; }
        public long ApplyForLoanId { get; set; }
        public decimal disbusedAmount { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string BankCode { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string nuban { get; set; }
        public bool status { get; set; }
        public bool HaveStartedRepayment { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ApplyForLoan ApplyForLoan { get; set; }
      //  public virtual ICollection<LoanDailyDeductionLog> LoanDailyDeductionLog { get; set; }
        //public virtual ICollection<LoanMonthlyDeductionLog> LoanMonthlyDeductionLog { get; set; }
    }

}
