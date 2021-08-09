using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class VendAirtimeDTO
    {
        [Required(ErrorMessage = "mobile number is required")]
        public string mobile { get; set; }
        [Required(ErrorMessage = "amount is required")]
        public string amt { get; set; }
        [Required(ErrorMessage = "paymentcode is required")]
        public string paymentcode { get; set; }

    }

    public class VendAirtimeRequestDto : VendAirtimeDTO
    {
        public string Referenceid { get; set; }
        public string Translocation { get; set; }
        public string email { get; set; }
        public string SubscriberInfo1 { get; set; }
        public string nuban { get; set; }
        public string TransactionType { get; set; }
        public int AppId { get; set; }
        public int RequestType { get; set; }
        public string TerminalID { get; set; }

    }
}
