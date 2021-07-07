using SocialPay.Core.Extensions.Utilities.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SocialPay.Core.Extensions.Common
{
    public static class StringExtensions
    {
        public static string GenerateHmac(this string message, string secretKey, bool toBase64, HmacType hmacType = HmacType.Sha256)
        {
            using var hasher = new HmacAlgorithm(hmacType);
            var hash = hasher.ComputeHash(message, secretKey);

            if (toBase64)
                return Convert.ToBase64String(hash);

            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    
    }
}
