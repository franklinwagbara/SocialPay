﻿namespace SocialPay.Helper.Dto.Request
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class PasswordResetDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class AccountResetDto
    {
        public string Email { get; set; }
      
    }
}
