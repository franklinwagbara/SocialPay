using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class UpdatePaymentDTO
    {

        [Required(ErrorMessage = "Payment Link Name")]
        public string PaymentLinkName { get; set; }
    }
}
