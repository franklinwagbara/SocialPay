using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SocialPay.Core.Extensions.Utilities.Cryptography
{
    public class HmacAlgorithm : IDisposable
    {
        private readonly HMAC _hasher;

        public HmacAlgorithm(HmacType hmacType) => _hasher = Create(hmacType);

        public byte[] ComputeHash(string message, string key)
        {
            _hasher.Key = Encoding.UTF8.GetBytes(key);

            var messageBytes = Encoding.UTF8.GetBytes(message);
            return _hasher.ComputeHash(messageBytes);
        }

        private static HMAC Create(HmacType hmacType)
        {
            return hmacType switch
            {
                HmacType.Md5 => new HMACMD5(),
                HmacType.Sha1 => new HMACSHA1(),
                HmacType.Sha256 => new HMACSHA256(),
                HmacType.Sha384 => new HMACSHA384(),
                HmacType.Sha512 => new HMACSHA512(),
                _ => throw new ArgumentException(nameof(hmacType)),
            };
        }

        public void Dispose() => _hasher.Dispose();
    }

    public enum HmacType
    {
        Md5,
        Sha1,
        Sha256,
        Sha384,
        Sha512
    }
}
