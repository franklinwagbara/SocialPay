using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class SignUpRequestDto
    {
        [Required(ErrorMessage = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "FullName")]
        public string Fullname { get; set; }
        [Required(ErrorMessage = "Date of birth")]
        public string DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender")]
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
}
