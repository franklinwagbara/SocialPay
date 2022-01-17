using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class LoanRepaymentModel
    {
        [Required(ErrorMessage = "DailySalesPercentage is required")]
        public double DailySalesPercentage { get; set; }
        [Required(ErrorMessage = "Rate is required")]
        public double Rate { get; set; }
        [Required(ErrorMessage = "PA is required")]
        public double PA { get; set; }
    }


    public class DeleteLoanRepaymentModel
    {
        [Required(ErrorMessage = "delete is required")]
        public bool delete { get; set; }

        [Required(ErrorMessage = "LoanRepaymentPlanId is required")]
        public long LoanRepaymentPlanId { get; set; }
    }
}
