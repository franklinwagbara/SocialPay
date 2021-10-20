using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class SpectaResponseWithBoolResultMessage
    {
        public class Error
        {
            //public long ErrorId { get; set; }
            public int code { get; set; }
            public string message { get; set; }
            public string details { get; set; }
            public string validationErrors { get; set; }
        }


        public class SpectaResponseDto
        {
            public bool result { get; set; }
            public string targetUrl { get; set; }
            public bool success { get; set; }
            public Error error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }
        }
    }


    public class SpectaResponseWithObjectResultMessage
    {
        public class Error
        {
            //public long ErrorId { get; set; }
            public int code { get; set; }
            public string message { get; set; }
            public string details { get; set; }
            public string validationErrors { get; set; }
        }


        public class SpectaResponseDto
        {
            public object result { get; set; }
            public string targetUrl { get; set; }
            public bool success { get; set; }
            public Error error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }
        }

    }
}
