using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class BulkSignUpRequestDto
    {
        [MaxLength(50, ErrorMessage = "Email cannot be greater than 50")]
        [Required(ErrorMessage = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Number")]
        [RegularExpression(@"^\d*[0-9]\d*$", ErrorMessage = "Only number between 0 - 9 allowed")]
        [MaxLength(20, ErrorMessage = "Phone Number cannot be greater than 20")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Phone Number")]
        [RegularExpression(@"^\d*[0-9]\d*$", ErrorMessage = "Only number between 0 - 9 allowed")]
        [MaxLength(11, ErrorMessage = "Bvn Number cannot be greater than 11")]
        public string Bvn { get; set; }
        [Required(ErrorMessage = "FirstName")]
        [MaxLength(55, ErrorMessage = "FirstName cannot be greater than 55")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName")]
        [MaxLength(55, ErrorMessage = "LastName cannot be greater than 55")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Date of birth")]
        [MaxLength(15, ErrorMessage = "Date of birth cannot be greater than 15")]
        public string DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender")]
        [MaxLength(10, ErrorMessage = "Gender cannot be greater than 10")]
        public string Gender { get; set; }

        // public string ReferralCode { get; set; }
        // public string Password { get; set; }
    }

}
