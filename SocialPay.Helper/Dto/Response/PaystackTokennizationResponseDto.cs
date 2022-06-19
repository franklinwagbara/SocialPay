using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{

    public class PaystackTokennizationResponseDto
    {
        public class Error
        {
            public int code { get; set; }
            public string message { get; set; }
            public object details { get; set; }
            public object validationErrors { get; set; }
        }


        public class Result
        {
            public string displayText { get; set; }
            public string status { get; set; }
            public string authUrl { get; set; }
            public string message { get; set; }
            public string cardStatus { get; set; }
            public string bankName { get; set; }
            public string cardId { get; set; }
            public bool isValidCardForStatement { get; set; }
            public bool isSterling { get; set; }
        }


        public class PaystackTokennizationResponse
        {
            public Result result { get; set; }
            public bool success { get; set; }
            public Error error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }
            public string ResponseCode { get; set; }
        }
    }
}
