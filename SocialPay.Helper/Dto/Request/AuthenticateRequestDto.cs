using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class AuthenticateRequestDto
    {
        public string userNameOrEmailAddress { get; set; }
        public string password { get; set; }
        public bool rememberClient { get; set; }
    }
}
