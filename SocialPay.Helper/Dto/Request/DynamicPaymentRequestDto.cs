using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class DynamicPaymentRequestDto
    {
        public string amount { get; set; }       
    }

    public class DynamicPaymentDefaultRequestDto : DynamicPaymentRequestDto
    {
        public string customerIdentifier { get; set; }
        public string orderSn { get; set; }
        public string userBankNo { get; set; }
        public string userAccountName { get; set; }
        public string userAccountNumber { get; set; }
        public string userBankVerificationNumber { get; set; }
        public string userKycLevel { get; set; }
        public string userGps { get; set; }
        public string mchNo { get; set; }
        public string subMchNo { get; set; }
    }
}
