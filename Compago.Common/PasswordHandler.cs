using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Compago.Common
{
    public static class PasswordHandler
    {
        public static (string hash, string salt) HashPassword(string password, string? salt = null)
        {
            byte[] saltBytes = salt != null ? Convert.FromBase64String(salt) : RandomNumberGenerator.GetBytes(128 / 8);

            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return (hash, Convert.ToBase64String(saltBytes));
        }

        public static bool ValidatePassword(string password, string passwordHash, string salt)
        {
            return HashPassword(password, salt).hash.Equals(passwordHash);
        }
    }
}
