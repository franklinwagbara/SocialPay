using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class DstvAccountLookupResponseDto
    {
        public long DstvAccountLookupResponseId { get; set; }
        public string merchantReference { get; set; }
        public string payUVasReference { get; set; }
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
        public string pointOfFailure { get; set; }
        public string merchantId { get; set; }
    }
}
