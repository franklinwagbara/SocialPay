using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{

    public class TenantResponseDTO
    {
        public DataClass data { get; set; }
        public string success { get; set; }
    }

    public class DataClass
    {
        public string responseCode { get; set; }
        public string userStatus { get; set; }
        public string message { get; set; }
        public Lv2Data data { get; set; }
    }
}
