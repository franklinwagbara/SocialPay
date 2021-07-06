using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class UpdatePaymentDTO
    {

        [Required(ErrorMessage = "Payment Link Name")]
        public string PaymentLinkName { get; set; }
    }
}
