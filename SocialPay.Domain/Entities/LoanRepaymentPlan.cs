using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class LoanRepaymentPlan : BaseEntity
    {
        public long LoanRepaymentPlanId { get; set; }
        public bool IsDeleted { get; set; }
        public double DailySalesPercentage { get; set; }
        public double Rate { get; set; }
        public double PA { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ICollection<ApplyForLoan> ApplyForLoan { get; set; }
    }
}
