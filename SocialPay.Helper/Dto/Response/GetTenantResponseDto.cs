using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{

    public class GetTenantResponseDto
    {
        public LvData data { get; set; }
    }

    public class LvData
    {
        public string responseCode { get; set; }
        public Lv2Data data { get; set; }
    }

    public class Lv2Data
    {
        public long tenantProfileId { get; set; }
        public string email { get; set; }
        public string tenantName { get; set; }
        public string phoneNumber { get; set; }
        public string address { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public string authKey { get; set; }
    }
}
