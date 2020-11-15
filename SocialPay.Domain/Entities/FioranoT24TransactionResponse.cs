using System;

namespace SocialPay.Domain.Entities
{
    public class FioranoT24TransactionResponse
    {
        public long FioranoT24TransactionResponseId { get; set; }
        public string PaymentReference { get; set; }
        public string ReferenceID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string Balance { get; set; }
        public string COMMAMT { get; set; }
        public string CHARGEAMT { get; set; }
        public string FTID { get; set; }
        public string JsonResponse { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        //public virtual FioranoT24Request FioranoT24Request { get; set; }
    }

}
