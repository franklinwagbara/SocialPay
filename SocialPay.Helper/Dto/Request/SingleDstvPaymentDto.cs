using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class SingleDstvPaymentDto
    {
        [Required(ErrorMessage = "Amount is required")]
        public string amountInCents { get; set; }
        public List<CustomField> customFields { get; set; }
      
        [Required(ErrorMessage = "Merchant Reference is required")]
        public string merchantReference { get; set; }
       // [Required(ErrorMessage = "Transaction Type is required")]        
        public string customerId { get; set; }

    }

    public class SingleDstvPaymentDefaultDto : SingleDstvPaymentDto
    {
        public string transactionType { get; set; }
        public string vasId { get; set; }
        public string countryCode { get; set; }

    }

    public class CustomField
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
