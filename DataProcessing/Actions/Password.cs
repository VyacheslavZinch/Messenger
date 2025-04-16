using Entities;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System.Security.Cryptography;

namespace DataProcessing.Actions
{
    public static class Password
    {
        public static string NewPasswordGenerator()
        {
            var randomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex
            {
                Pattern = @"^((?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9]).{6,})\S$"
            });
            return randomizer.Generate();
        }

        public static byte[] GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static string SaltBase64(byte[] salt) => Convert.ToBase64String(salt);
        public static byte[] HashPassword(string password, byte[] salt, int iterations = 100_000, int hashByteSize = 32)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(hashByteSize);
            }
        }

        public static string HashPasswordBase64(string password, byte[] salt)
        {
            byte[] hash = HashPassword(password, salt);
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string inputPassword, string storedSaltBase64, string storedHashBase64)
        {
            byte[] salt = Convert.FromBase64String(storedSaltBase64);
            byte[] hash = HashPassword(inputPassword, salt);
            string inputHashBase64 = Convert.ToBase64String(hash);

            return inputHashBase64 == storedHashBase64;
        }




    }
}
