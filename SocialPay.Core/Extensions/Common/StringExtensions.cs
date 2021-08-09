using SocialPay.Core.Extensions.Utilities.Cryptography;
using System;
using System.IO;
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


        public static String DecryptAlt(String val)
        {
            var ms = new MemoryStream();

            byte[] sharedkey = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11, 0x12, 0x11, 0x0D, 0x0B, 0x07, 0x02, 0x04, 0x08, 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };
            byte[] sharedvector = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };

            var tdes = new TripleDESCryptoServiceProvider();

            byte[] toDecrypt = Convert.FromBase64String(val);

            var cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);

            cs.Write(toDecrypt, 0, toDecrypt.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
    
}
