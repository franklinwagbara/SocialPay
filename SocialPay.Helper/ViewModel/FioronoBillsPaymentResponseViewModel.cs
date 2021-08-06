using System;

namespace SocialPay.Helper.ViewModel
{
    public class FioronoBillsPaymentResponseViewModel
    {
        public long FioranoBillsPaymentResponseId { get; set; }
        public long FioranoBillsRequestId { get; set; }
        public string PaymentReference { get; set; }
        public string ReferenceID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string Balance { get; set; }
        public string COMMAMT { get; set; }
        public string CHARGEAMT { get; set; }
        public string FTID { get; set; }
        public string JsonResponse { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
