using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class UpdateMerchantPersonalInfoRequestDto
    {
        [MaxLength(50, ErrorMessage = "Email cannot be greater than 50")]
        [Required(ErrorMessage = "Email")]
        public string Email { get; set; }
        // [Required(ErrorMessage = "Phone Number")]
        [RegularExpression(@"^\d*[0-9]\d*$", ErrorMessage = "Only number between 0 - 9 allowed")]
        [MaxLength(20, ErrorMessage = "Phone Number cannot be greater than 20")]
        public string PhoneNumber { get; set; }
        [RegularExpression(@"^\d*[0-9]\d*$", ErrorMessage = "Only number between 0 - 9 allowed")]
        [MaxLength(11, ErrorMessage = "Bvn Number cannot be greater than 11")]
        public string Bvn { get; set; }
        [MaxLength(55, ErrorMessage = "Full name cannot be greater than 55")]
        public string FullName { get; set; }

    }

    public class MerchantUpdateInfoRequestDto
    {
        [MaxLength(65, ErrorMessage = "Business name cannot be greater than 65")]
        public string BusinessName { get; set; }
        [MaxLength(20, ErrorMessage = "Business Phone number can not be greater than 20")]
        [RegularExpression(@"^\d*[0-9]\d*$", ErrorMessage = "Only number between 0 - 9 allowed")]
        public string BusinessPhoneNumber { get; set; }
        [MaxLength(40, ErrorMessage = "Business Email can not be greater than 40")]
        public string BusinessEmail { get; set; }
        [MaxLength(40, ErrorMessage = "Tin can not be greater than 40")]
        public string Tin { get; set; }
        public string Country { get; set; }
        public string Chargebackemail { get; set; }
        //[Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile Logo { get; set; }
    }

}
