using System;
using System.Security.Cryptography;
using System.Text;

namespace Hero.AutoTrading.Bitkub.Utils
{
    public static class CryptographyUtil
    {
        public static string HashHMACSHA256(string secretKey, string message)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);
            var buffer = Encoding.ASCII.GetBytes(message);
            var sha256 = new HMACSHA256(key);
            var byteHash = sha256.ComputeHash(buffer);
            var hexMessage = Convert.ToHexString(byteHash).ToLower();
            return hexMessage;
        }
    }
}
