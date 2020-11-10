using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password")]
        public string Password { get; set; }
    }

    public class PasswordResetDto
    {
        [Required(ErrorMessage = "Token")]
        public string Token { get; set; }
        [Required(ErrorMessage = "New Password")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AccountResetDto
    {
        [Required(ErrorMessage = "Email")]
        public string Email { get; set; }
      
    }
}
