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
            public double loanLimit { get; set; }
            public string registrationStage { get; set; }
            public double remainingLimit { get; set; }
            public DateTime remainingLimitExpiryDate { get; set; }
            public DateTime applicableForLimitOn { get; set; }
            public string disbursementAccountNumber { get; set; }
            public string userType { get; set; }
            public List<object> purchases { get; set; }
            public int id { get; set; }
        }

        public class Error
        {
            public int code { get; set; }
            public string message { get; set; }
            public string details { get; set; }
            public string validationErrors { get; set; }
        }

        public class LoggedInCustomerProfileResponse
        {
            public Result result { get; set; }
            public string targetUrl { get; set; }
            public bool success { get; set; }
            public Error error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }
        }
    }
}
