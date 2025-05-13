using DataProcessing.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataProcessing.Controllers
{
    /*
     * Создаём сервис Redis
     */
    public class RedisTokenAuthorizationHandler : AuthorizationHandler<RedisTokenRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RedisData _redisData;

        public RedisTokenAuthorizationHandler(IHttpContextAccessor httpContextAccessor, RedisData redisData)
        {
            _httpContextAccessor = httpContextAccessor;
            _redisData = redisData;
        }

        /**/
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RedisTokenRequirement requirement)
        {
            Console.WriteLine("RedisTokenAuthorizationHandler: Starting authorization...");

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                Console.WriteLine("RedisTokenAuthorizationHandler: HttpContext is null");
                context.Fail();
                return;
            }

            // Извлекаем access_token из query-параметров (для SignalR)
            string accessToken = httpContext.Request.Query["access_token"];
            Console.WriteLine($"RedisTokenAuthorizationHandler: Access token from query = {accessToken}");

            // Если access_token отсутствует, извлекаем токен из  заголовка Authorization
            if (string.IsNullOrEmpty(accessToken))
            {
                var authHeader = httpContext.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    accessToken = authHeader.Substring("Bearer ".Length).Trim();
                    Console.WriteLine($"RedisTokenAuthorizationHandler: Access token from header = {accessToken}");
                }
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("RedisTokenAuthorizationHandler: No access token found");
                context.Fail();
                return;
            }

            // Пробуем извлечь userId из query
            string userId = httpContext.Request.Query["userId"];
            Console.WriteLine($"RedisTokenAuthorizationHandler: UserId from query = {userId}");

            // Если userId нет в query, пробуем извлечь из JWT-токена
            if (string.IsNullOrEmpty(userId))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(accessToken);
                    userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    Console.WriteLine($"RedisTokenAuthorizationHandler: UserId from JWT = {userId}");
                }
                catch
                {
                    Console.WriteLine("RedisTokenAuthorizationHandler: Failed to parse JWT token");
                }
            }

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("RedisTokenAuthorizationHandler: No userId found");
                context.Fail();
                return;
            }

            // Извлекаем токен из Redis
            var tokenFromRedis = await _redisData.GetTokenAsync(userId);
            Console.WriteLine($"RedisTokenAuthorizationHandler: Token from Redis = {tokenFromRedis}");

            if (string.IsNullOrEmpty(tokenFromRedis))
            {
                Console.WriteLine($"RedisTokenAuthorizationHandler: No token found in Redis for userId = {userId}");
                context.Fail();
                return;
            }

            // Сравниваем токены
            if (tokenFromRedis != accessToken)
            {
                Console.WriteLine($"RedisTokenAuthorizationHandler: Token mismatch: request token = {accessToken}, Redis token = {tokenFromRedis}");
                context.Fail();
                return;
            }

            // Если токены совпадают, добавляем userId в ClaimsPrincipal
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userId)
            };
            var identity = new ClaimsIdentity(claims, "RedisToken");
            context.User.AddIdentity(identity);

            Console.WriteLine("RedisTokenAuthorizationHandler: Authorization successful");
            context.Succeed(requirement);
        }
    }

    public class RedisTokenRequirement : IAuthorizationRequirement
    {
    }
}