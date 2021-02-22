using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class SignUpRequestDto
    {
        [Required(ErrorMessage = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Number")]
        [RegularExpression(@"^\d*[0-9]\d*$", ErrorMessage = "Only number between 0 - 9 allowed")]
        [MaxLength(15, ErrorMessage = "Phone Number cannot be greater than 15")]
        public long PhoneNumber { get; set; }
        [Required(ErrorMessage = "FullName")]
        [MaxLength(45, ErrorMessage = "Full name cannot be greater than 45")]
        public string Fullname { get; set; }
        [Required(ErrorMessage = "Date of birth")]
        [MaxLength(15, ErrorMessage = "Date of birth cannot be greater than 15")]
        public string DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender")]
        [MaxLength(10, ErrorMessage = "Gender cannot be greater than 10")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }
    }

    public class SignUpConfirmationRequestDto
    {
       // [Required(ErrorMessage = "Pin is required.")]
        public string Pin { get; set; }
       // [Required(ErrorMessage = "Token is required.")]
       //[JsonIgnore]
       public string Token { get; set; }
    }


    public class UpdateUserRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        public bool Status { get; set; }
    }

    public class GuestAccountRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
    }
}
