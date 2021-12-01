using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class CradTokenizationResponseDTO
    {
        public DataResponse data { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public int code { get; set; }
    }

    public class CradTokenizationInClassResponse
    {
        public bool status { get; set; }
        public string redirectUrl { get; set; }
    }


    public class DataResponse
    {
        public string reference { get; set; }
        public string url { get; set; }
    }
}
