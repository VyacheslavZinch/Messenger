using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Security.Cryptography;

namespace DataProcessing.Actions
{
    public static class Password
    {
        /*
         * генерация нового пароля для пользователя
         * используется в контроллере восстановления доступа к аккаунту
         */
        public static string NewPasswordGenerator()
        {
            var randomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex
            {
                Pattern = @"^[A-Za-z0-9=\!\@#\%\^\&\*\-\?\[\]\|]{9,}$"
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
        //Хеширование пароля
        public static byte[] HashPassword(string password, byte[] salt, int iterations = 100_000, int hashByteSize = 32)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(hashByteSize);
            }
        }

        //Преобразование массива байт соли в строку для хранения в БД
        public static string SaltBase64(byte[] salt) => Convert.ToBase64String(salt);

        //Хешируем пароль, преобразовываем в строку для хранения в БД
        public static string HashPasswordBase64(string password, byte[] salt)
        {
            byte[] hash = HashPassword(password, salt);
            return Convert.ToBase64String(hash);
        }

        //Сравниваем хеши паролей (полученного от пользователя и пароля из БД)
        public static bool VerifyPassword(string inputPassword, string storedSaltBase64, string storedHashBase64)
        {
            try
            {
                byte[] salt = Convert.FromBase64String(storedSaltBase64);

                byte[] hash = HashPassword(inputPassword, salt, 100_000);

                string inputHashBase64 = Convert.ToBase64String(hash);
#if DEBUG
                Console.WriteLine($"Stored Hash: {storedHashBase64}");
                Console.WriteLine($"Computed Hash: {inputHashBase64}");
#endif

                return inputHashBase64 == storedHashBase64;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
