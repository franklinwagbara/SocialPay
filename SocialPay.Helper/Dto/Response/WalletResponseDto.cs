using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class Data
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public object email { get; set; }
        public string mobile { get; set; }
    }

    public class WalletResponseDto
    {
        public string message { get; set; }
        public string response { get; set; }
        public object responsedata { get; set; }
        public Data data { get; set; }
    }
}
