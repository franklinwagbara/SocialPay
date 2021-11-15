using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class StoreRequestDto
    {
        [Required(ErrorMessage = "Please enter store.")]
        [StringLength(30, ErrorMessage = "Cannot exceed 30 characters. ")]
        public string StoreName { get; set; }
        [Required(ErrorMessage = "Please enter link.")]
        [StringLength(30, ErrorMessage = "Cannot exceed 30 characters. ")]
        public string StoreLink { get; set; }
        [StringLength(150, ErrorMessage = "Cannot exceed 30 characters. ")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please select image.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", ".pdf", ".svg" })]
        public IFormFile Image { get; set; }
    }
}
