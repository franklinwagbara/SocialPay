using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class LoggedInCustomerProfileResponseDto
    {
        public class Result
        {
            public string spectaId { get; set; }
            public string businessId { get; set; }
            public string bvn { get; set; }
            public string name { get; set; }
            public string surname { get; set; }
            public string userName { get; set; }
            public string emailAddress { get; set; }
            public bool hasTokenizedCard { get; set; }
            public object loanLimit { get; set; }
            public string registrationStage { get; set; }
            public object remainingLimit { get; set; }
            public object remainingLimitExpiryDate { get; set; }
            public DateTime applicableForLimitOn { get; set; }
            public object disbursementAccountNumber { get; set; }
            public string userType { get; set; }
            public List<object> purchases { get; set; }
            public int id { get; set; }
        }

        public class LoggedInCustomerProfileResponse
        {
            public Result result { get; set; }
            public object targetUrl { get; set; }
            public bool success { get; set; }
            public object error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }
        }
    }
}
