﻿namespace SocialPay.Helper.Dto.Request
{
    public class SignUpRequestDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Fullname { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class SignUpConfirmationRequestDto
    {
       // [Required(ErrorMessage = "Pin is required.")]
        public string Pin { get; set; }
       // [Required(ErrorMessage = "Token is required.")]
       ///[JsonIgnore]
       // public string Token { get; set; }
    }
}
