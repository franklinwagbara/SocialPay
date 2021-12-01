using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class ApplyForLoanResponseDTO
    {
        public string RedirectUrl { get; set; }
        public long ApplyForLoanId { get; set; }
    }

    public class AppliedLoanStatus
    {
        public decimal Amount { get; set; }
        public bool IsAttended { get; set; }
        public bool IsApproved { get; set; }
        public bool IsBadDebt { get; set; }
        public bool IsCardTokenized { get; set; }
        public bool isCustomerClean { get; set; }
    }
}
