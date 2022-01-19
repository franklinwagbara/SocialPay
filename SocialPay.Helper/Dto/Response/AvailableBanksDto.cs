using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class AvailableBanksDto
    {
        public class Result
        {
            public string id { get; set; }
            public string name { get; set; }
            public string sortCode { get; set; }
        }

        public class AvailableBanksDtoResponse
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
