using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class ApplyForloanRequestDTO
    {
        public decimal Amount { get; set; }

        public long LoanRepaymentPlanId { get; set; }
        public string redirectUrl { get; set; }
    }


    public class AdminLoanApproverRequestDTO
    {
        public decimal ApprovedAmount { get; set; }

        public long ApplyForLoanId { get; set; }

        public bool IsApproved { get; set; }
    }

    public class ConfirmTokenizationRequestDTO
    {
        public string Ref { get; set; }
        public long ApplyForLoanId { get; set; }

    }
    public class MerchantEligibilty
    {

        public bool IsExitingLoan { get; set; }
        public decimal Amount { get; set; }
    }
}
