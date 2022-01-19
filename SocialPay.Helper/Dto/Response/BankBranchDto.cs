using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class BankBranchDto
    {
        public class Result
        {
            public string branchCode { get; set; }
            public string branchName { get; set; }
        }
        public class BankBranchDtoResponse
        {
            public List<Result> result { get; set; }
            public object targetUrl { get; set; }
            public bool success { get; set; }
            public object error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }
        }
    }
}
