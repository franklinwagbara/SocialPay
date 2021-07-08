using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class DynamicPaymentResponseDto
    {
        public string nameEnquiryRef { get; set; }
        public string destinationInstitutionCode { get; set; }
        public string channelCode { get; set; }
        public string beneficiaryAccountName { get; set; }
        public string beneficiaryAccountNumber { get; set; }
        public string beneficiaryKYCLevel { get; set; }
        public string beneficiaryBankVerificationNumber { get; set; }
        public string originatorAccountName { get; set; }
        public string originatorAccountNumber { get; set; }
        public string originatorBankVerificationNumber { get; set; }
        public string originatorKYCLevel { get; set; }
        public string transactionLocation { get; set; }
        public string narration { get; set; }
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
        public string paymentReference { get; set; }
        public string amount { get; set; }
        public string sessionID { get; set; }
        public string ResponseCode { get; set; }
        public string jsonResponse { get; set; }
    }
}
