using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Project.Service.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// Generate hash SHA512 from string
        /// </summary>
        /// <returns>The 512 hash.</returns>
        /// <param name="inputString">Input string.</param>
        public static string SHA512Hash(this string inputString)
        {
            // Create byte from input string
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);

            // Crete hash from bytes
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            // Return hash value
            return result.ToString();
        }


        /// <summary>
        /// Checks if string is valid email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            // source: http://thedailywtf.com/Articles/Validating_Email_Addresses.aspx
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }
    }
}
