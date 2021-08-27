using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class GenerateReferenceDTO
    {
        [Required(ErrorMessage = "Amount is required")]
        public string amount { get; set; }

    }

    public class GenerateReferenceRequestDTO : GenerateReferenceDTO
    {
        public string merchantID { get; set; }
        public string channel { get; set; }
        public string transactionType { get; set; }
        public string transRef { get; set; }
        public string merchantName { get; set; }
        public string callbackUrl { get; set; }
    }

    public class GatewayRequeryDTO
    {
        //[Required(ErrorMessage = "TransactionID is required")]
       // public string PaymentReference { get; set; }
        public string TransactionID { get; set; }
    }

    public class GatewayRequeryRequestDTO : GatewayRequeryDTO
    {
        public string amount { get; set; }
        public string merchantID { get; set; }
        public string terminalID { get; set; }
    }

    public class CallbackDTO
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string TransactionRef { get; set; }


    }
}
