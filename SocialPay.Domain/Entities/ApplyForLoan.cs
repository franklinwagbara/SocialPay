using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ApplyForLoan : BaseEntity
    {
        public long ApplyForLoanId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long LoanRepaymentPlanId { get; set; }
        public decimal Amount { get; set; }
        public bool IsAttended { get; set; }
        public bool IsApproved { get; set; }
        public bool IsBadDebt { get; set; }
        public bool IsCardTokenized { get; set; }
        [Column(TypeName = "NVARCHAR(130)")]
        public string TokenizationToken { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string TokenizationEmail { get; set; }
        public bool isCustomerClean { get; set; }
        public bool HaveSterlingBankAccount{ get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string ConfirmTokenizationResponse { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TokenizationReference { get; set; }
        public bool HaveSterlingBankBusinessAccount { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
        public virtual LoanRepaymentPlan LoanRepaymentPlan { get; set; }
        public virtual ICollection<LoanDisbursement> LoanDisbursement { get; set; }
        public virtual ICollection<LoanDailyDeductionLog> LoanDailyDeductionLog { get; set; }
        public virtual ICollection<LoanMonthlyDeductionLog> LoanMonthlyDeductionLog { get; set; }
    }

}
