using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class NipFundstransferRequestDto
    {
        public string OriginatorKYCLevel { get; set; }
        public string BeneficiaryKYCLevel { get; set; }
        public string BeneficiaryBankVerificationNumber { get; set; }
        public string OriginatorBankVerificationNumber { get; set; }
        public int AppID { get; set; }
        public string AccountLockID { get; set; }
        public string OriginatorAccountNumber { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string DestinationBankCode { get; set; }
        public string OrignatorName { get; set; }
        public string SubAcctVal { get; set; }    
        public string LedCodeVal { get; set; }    
        public string CurCodeVal { get; set; }    
        public string CusNumVal { get; set; }    
        public string BraCodeVal { get; set; }
        public double Vat { get; set; }            
        public decimal Fee { get; set; }            
        public decimal Amount { get; set; }            
        public string PaymentRef { get; set; }            
        public string NESessionID { get; set; }            
        public string ChannelCode { get; set; }            
         
    }
}
