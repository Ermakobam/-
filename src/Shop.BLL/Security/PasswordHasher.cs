using System;
using System.Security.Cryptography;
using System.Text;

namespace Shop.BLL.Security
{
    public interface IPasswordHasher
    {
        string Hash(string rawPassword);
    }

    public class Base64PasswordHasher : IPasswordHasher
    {
        public string Hash(string rawPassword)
        {
            if (string.IsNullOrEmpty(rawPassword))
            {
                return Convert.ToBase64String(Array.Empty<byte>());
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(rawPassword));
        }
    }

    public static class PasswordHashes
    {
        public static string Sha256(string rawPassword)
        {
            string value = rawPassword ?? "";
            if (value.Length == 64 && IsHex(value))
            {
                return value.ToLowerInvariant();
            }

            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static bool IsHex(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            foreach (char c in value)
            {
                if (!char.IsAsciiHexDigit(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
