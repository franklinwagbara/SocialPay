﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper
{
    public class AppResponseCodes
    {
        public const string Success = "00";
        public const string InternalError = "02";
        public const string Failed = "03";
        public const string SignUp = "04";
        public const string DuplicateEmail = "05";
        public const string RecordNotFound = "06";
        public const string InvalidLogin = "07";
               
    }



    public class RoleDetails
    {
        public const string Merchant = "Merchant";
        public const string SuperAdministrator = "Super Administrator";
        public const string Administrator = "Administrator";     
    }
}
