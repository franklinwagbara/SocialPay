using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class DisputeItemRequestDto
    {
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string Comment { get; set; }
        [Required(ErrorMessage = "Document is required")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", "pdf" })]
        public IFormFile Document { get; set; }
    }
}
