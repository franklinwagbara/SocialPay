using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class BindMechantResponseDto
    {
        public string ReturnCode { get; set; }
        public string Mch_no { get; set; }
        public string ResponseCode { get; set; }
        public string jsonResponse { get; set; }
    }
}
