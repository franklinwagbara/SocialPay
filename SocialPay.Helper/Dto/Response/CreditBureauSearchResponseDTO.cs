using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class CreditBureauSearchResponseDTO
    {
        public creditSearchResponse creditSearchResponse { get; set; }
        public object badConsumerReport { get; set; }
        public object crcSearchRequest { get; set; }
        public object consumerFullCreditReport { get; set; }
        public object mergedConsumersReport { get; set; }

    }
    public class creditSearchResponse
    {
        public bool isCustomerClean { get; set; }
        public string remarks { get; set; }
        public string error { get; set; }
        public int statusCode { get; set; }
    }
}
