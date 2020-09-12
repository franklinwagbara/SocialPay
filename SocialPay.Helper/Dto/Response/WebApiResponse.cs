using System;

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
        public string UserStatus { get; set; }
        public string AccessToken { get; set; }
        public string Role { get; set; }
        public string BusinessName { get; set; }
    }
}
