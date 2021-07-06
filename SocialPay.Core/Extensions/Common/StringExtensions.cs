using SocialPay.Core.Extensions.Utilities.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Core.Extensions.Common
{
    public static class StringExtensions
    {
        public static string GenerateHmac(this string message, string secretKey, HmacType hmacType = HmacType.Sha1)
        {
            using var hasher = new HmacAlgorithm(hmacType);
            var hash = hasher.ComputeHash(message, secretKey);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}
