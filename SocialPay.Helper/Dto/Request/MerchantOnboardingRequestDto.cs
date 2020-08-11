using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class MerchantOnboardingRequestDto
    {
        [Required(ErrorMessage = "Business Name")]
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "Business Phone number")]
        public string BusinessPhoneNumber { get; set; }
        [Required(ErrorMessage = "Business email")]
        public string BusinessEmail { get; set; }
        [Required(ErrorMessage = "Business country")]
        public string Country { get; set; }
        public string Chargebackemail { get; set; }
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile Logo { get; set; }
    }
}
