﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class BulkSignUpResponseDto
    {
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string ResponseCode { get; set; }
    }
}
