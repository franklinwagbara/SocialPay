using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class ValidateChargeRequestDto
    {
        public string cardId { get; set; }
        public string Email { get; set; }
    }
}
