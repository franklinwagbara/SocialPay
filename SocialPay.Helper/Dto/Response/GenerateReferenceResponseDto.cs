using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class GenerateReferenceResponseDto
    {
        public string ResponseCode { get; set; }
        public ResponseHeader ResponseHeader { get; set; }
        public ResponseDetails ResponseDetails { get; set; }
    }

    public class ResponseHeader
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }

    public class ResponseDetails
    {
        public string Reference { get; set; }
        public string Amount { get; set; }
        public string TransactionID { get; set; }
        public string TraceID { get; set; }
    }
}
