using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class StoreRequestDto
    {
        [Required(ErrorMessage = "Please enter store.")]
        public string StoreName { get; set; }
        [Required(ErrorMessage = "Please enter link.")]
        public string StoreLink { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Please select image.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", ".pdf", ".svg" })]
        public IFormFile Image { get; set; }
    }
}
