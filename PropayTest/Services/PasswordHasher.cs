using System.Text;
using System.Security.Cryptography;
using System;
namespace PropayTest.Services
{
    public class PasswordHasher
    {
        
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }



        }

        
        /// <param name="inputPassword">The plaintext password to verify.</param>
        /// <param name="storedHash">The stored hash to compare against.</param>
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            // Hash the input password and compare it with the stored hash.
            string inputHash = HashPassword(inputPassword);
            return inputHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}







