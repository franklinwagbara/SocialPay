using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class UssdRequestViewModel
    {
        public long UssdServiceRequestLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string MerchantID { get; set; }
        public string Channel { get; set; }
        public string TransactionType { get; set; }
        public string TransRef { get; set; }
        public string MerchantName { get; set; }
        public double Amount { get; set; }
        public string TerminalId { get; set; }
        public string RetrievalReference { get; set; }
        public string InstitutionCode { get; set; }
        public string ShortName { get; set; }
        public string Customer_mobile { get; set; }
        public string SubMerchantName { get; set; }
        public string UserID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string CallBackResponseCode { get; set; }
        public string CallBackResponseMessage { get; set; }
        public string TransactionRef { get; set; }
        public string TransactionID { get; set; }
        public string PaymentReference { get; set; }
        public string TraceID { get; set; }
        public DateTime DateEntered { get; set; }
        public DateTime LastDateModified { get; set; }
    }
  
}
