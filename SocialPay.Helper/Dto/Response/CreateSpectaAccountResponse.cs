using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class CreateSpectaAccountResponse
    {
        public bool success { get; set; }
        public error error { get; set; }
    }


    public class error
    {
        public int code { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public string validationErrors { get; set; }
    }
}
