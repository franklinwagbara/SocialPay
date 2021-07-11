using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class StoreRequestDto
    {
        public string StoreName { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Please select image.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", ".pdf", ".svg" })]
        public IFormFile Image { get; set; }
    }
}
