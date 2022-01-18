using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class ConfirmTicketResponseDto
    {
        public class Result
        {
            public string code { get; set; }
            public string shortDescription { get; set; }
            public double loanLimit { get; set; }
            public DateTime limitExpiryDate { get; set; }
        }


        public class Error
        {
            public int code { get; set; }
            public string message { get; set; }
            public string details { get; set; }
            public string validationErrors { get; set; }
        }



        public class ConfirmTicketResponse
        {
            public Result result { get; set; }
            public string targetUrl { get; set; }
            public bool success { get; set; }
            public Error error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }
            public string ResponseCode { get; set; }

        }
    }

}
