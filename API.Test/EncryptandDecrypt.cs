using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace API.Test
{
    public class EncryptandDecrypt
    {
        //private readonly IOptions<AppSettings> appSettings;
      

        public static string BinaryToString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                throw new ArgumentNullException("binary");

            if ((binary.Length % 8) != 0)
                throw new ArgumentException("Binary string invalid (must divide by 8)", "binary");

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < binary.Length; i += 8)
            {
                string section = binary.Substring(i, 8);
                int ascii = 0;
                try
                {
                    ascii = Convert.ToInt32(section, 2);
                }
                catch
                {
                    throw new ArgumentException("Binary string contains invalid section: " + section, "binary");
                }
                builder.Append((char)ascii);
            }
            return builder.ToString();
        }

        public String EncryptAlt(String val)
        {
            MemoryStream ms = new MemoryStream();
            string rsp = "";
            try
            {
                string sharedkeyval = "";
                string sharedvectorval = "";
                sharedkeyval = "000000010000001000000101000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100001100000000110000010100000111000010110000110100011011";
                sharedkeyval = BinaryToString(sharedkeyval);

                sharedvectorval = "0000000100000010000000110000010100000111000010110000110100010001";
                sharedvectorval = BinaryToString(sharedvectorval);

                byte[] sharedkey = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11, 0x12, 0x11, 0x0D, 0x0B, 0x07, 0x02, 0x04, 0x08, 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };
                byte[] sharedvector = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                byte[] toEncrypt = Encoding.UTF8.GetBytes(val);

                CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
                cs.Write(toEncrypt, 0, toEncrypt.Length);
                cs.FlushFinalBlock();
            }
            catch
            {
                return "";
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        /*public static String DecryptAlt(String val)
        {
            MemoryStream ms = new MemoryStream();

            string sharedkeyval = "";
            string sharedvectorval = "";

            sharedkeyval = "000000010000001000000101000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100001100000000110000010100000111000010110000110100011011";
            sharedkeyval = BinaryToString(sharedkeyval);

            sharedvectorval = "0000000100000010000000110000010100000111000010110000110100010001";
            sharedvectorval = BinaryToString(sharedvectorval);
            byte[] sharedkey = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11, 0x12, 0x11, 0x0D, 0x0B, 0x07, 0x02, 0x04, 0x08, 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };
            byte[] sharedvector = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toDecrypt = Convert.FromBase64String(val);

            CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);

            cs.Write(toDecrypt, 0, toDecrypt.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }*/
        public string Repad(string base64)
        {
            var l = base64.Length;
            return l % 4 == 1 && base64[l - 1] == '='
                ? base64.Substring(0, l - 1)
                : base64;
        }
        public String DecryptAlt(String val)
        {
            MemoryStream ms = new MemoryStream();
            string rsp = "";
            byte[] sharedkey = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11, 0x12, 0x11, 0x0D, 0x0B, 0x07, 0x02, 0x04, 0x08, 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };
            byte[] sharedvector = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toDecrypt = Convert.FromBase64String(val);

            CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);


            cs.Write(toDecrypt, 0, toDecrypt.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

    }
}
