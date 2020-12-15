using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SocialPay.Helper.Dto.Response
{

    public class TinResponseData
    {
        public string tin { get; set; }
        public string jtbtin { get; set; }
        public string taxPayerName { get; set; }
        public string address { get; set; }
        public string taxOfficeID { get; set; }
        public string taxOfficeName { get; set; }
        public string taxPayerType { get; set; }
        public string rcNumber { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class TinValidationResponseDto
    {
        public bool status { get; set; }
        public string message { get; set; }
        [JsonProperty("data")]
        public TinResponseData data { get; set; }
    }
}
