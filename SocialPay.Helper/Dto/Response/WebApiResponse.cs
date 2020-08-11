using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class WebApiResponse
    {
        public string ResponseCode { get; set; }
        public Object Data { get; set; }
    }


    public class LoginAPIResponse
    {
        public string ClientId { get; set; }
        public string ResponseCode { get; set; }
        public string AccessToken { get; set; }
        public string Role { get; set; }
    }
}
