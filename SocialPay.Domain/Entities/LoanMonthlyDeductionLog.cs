using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class LoanMonthlyDeductionLog : BaseEntity
    {
        public long LoanMonthlyDeductionLogId { get; set; }
        public long LoanDisbursementId { get; set; }
        public long ApplyForLoanId { get; set; }
        public double RepaymentAmount { get; set; }
        public double AmountDeducted { get; set; }
        public double DefaultBalance { get; set; }
        public virtual ApplyForLoan ApplyForLoan { get; set; }
      //  public virtual LoanDisbursement LoanDisbursement { get; set; }
    }

}
