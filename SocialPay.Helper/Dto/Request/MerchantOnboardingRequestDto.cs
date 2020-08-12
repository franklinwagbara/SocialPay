using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class MerchantOnboardingInfoRequestDto
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

    public class MerchantBankInfoRequestDto
    {
        public string BankName { get; set; }
        public string Nuban { get; set; }
        public string Currency { get; set; }
        public string BVN { get; set; }
        public string Country { get; set; }
        public bool DefaultAccount { get; set; }
    }
}
