using DotNetEnv;
using StackExchange.Redis;
using System;

namespace DataProcessing.Actions
{
    public class RedisData
    {
        /*
         * Лениво инициализируем значения статических переменных.
         * Переменные будут инициализированы перед первым обращением к ним.
         */

        private static readonly Lazy<string> RedisUser = new(() =>
        {
            return Environment.GetEnvironmentVariable("REDIS_USER");
        });

        private static readonly Lazy<string> RedisPassword = new(() =>
        {
            return Environment.GetEnvironmentVariable("REDIS_PASSWORD");
        });

        private static readonly Lazy<string> RedisEndpoint = new(() =>
        {
            return Environment.GetEnvironmentVariable("REDIS_HOST");
        });


        //создаём конфигурацию для Redis 
        public static readonly ConfigurationOptions RedisConfig = new ConfigurationOptions
        {
            EndPoints = { $"{RedisEndpoint.Value}:6380" },
            User = RedisUser.Value,
            Password = RedisPassword.Value,
            Ssl = false,
            AbortOnConnectFail = false
        };

        private readonly IDatabase _db;

        public RedisData(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        /*
         * устанавливаем Bearer Token, которым будем подписывать запросы
         * токен устанавливается при аутентификации пользователя и действует до момента,
         * пока пользователь не закончит сессию работы в приложении,
         * где в контроллере явно будет вызван метод удаления токена из Redis.
         * TTL токена не установлен для удобства поддержания работоспособности сесси
         * пользователя, так как токен придётся периодически обновлять и юзера может вылогинить
         */
        public async Task<bool> SetTokenAsync(string userId, string token)
        {
            return await _db.HashSetAsync("bearer-tokens", userId, token);
        }


        public async Task<string?> GetTokenAsync(string userId)
        {
            var token = await _db.HashGetAsync("bearer-tokens", userId);
            return token.IsNull ? null : token.ToString();
        }

        public async Task<bool> DeleteTokenAsync(string userId)
        {
            return await _db.HashDeleteAsync("bearer-tokens", userId);
        }
    }
}