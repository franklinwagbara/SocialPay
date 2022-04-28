using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class InterswitchTransactionVerificationDTO
    {
        public double amount { get; set; }
        public string cardNumber { get; set; }
        public string merchantReference { get; set; }
        public string paymentReference { get; set; }
        public string retrievalReferenceNumber { get; set; }
        public string[] splitAccounts { get; set; }
        public string transactionDate { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public string bankCode { get; set; }
        public string remittanceAmount { get; set; }


    }
}
