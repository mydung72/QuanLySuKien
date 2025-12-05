using System.Security.Cryptography;
using System.Text;

namespace EventBookingWeb.Helpers
{
    public static class PaymentHelper
    {
        public static string GenerateBookingCode()
        {
            return $"BK{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        public static string GenerateTicketCode()
        {
            return $"TK{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        public static string CreateMD5Hash(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static string CreateHMACHash(string key, string data)
        {
            var encoding = new UTF8Encoding();
            var keyBytes = encoding.GetBytes(key);
            var dataBytes = encoding.GetBytes(data);
            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}

