using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class BusinessSegmentAllListResponseDto
    {
        public class Result
        {
            public int id { get; set; }
            public string name { get; set; }
        }


        public class Error
        {
            public int code { get; set; }
            public string message { get; set; }
            public string details { get; set; }
            public string validationErrors { get; set; }
        }

        public class BusinessSegmentAllListResponse
        {
            public List<Result> result { get; set; }
            public object targetUrl { get; set; }
            public bool success { get; set; }
            public Error error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }

        }
    }

}
