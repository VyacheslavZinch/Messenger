using DataProcessing.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DataProcessing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RedisTokenPolicy")]
    public class UsersController : ControllerBase
    {

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
#if DEBUG
            Console.WriteLine(query);
#endif
            if (string.IsNullOrEmpty(query))
            {
                Console.WriteLine("UsersController: Search query is empty");
                return BadRequest("Query parameter is required.");
            }

            try
            {
                /*
                 * Получаем UserId из ClaimsPrincipal
                 * UserId необходим для проверки авторизации пользователя
                 */
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Console.WriteLine("UsersController: UserId not found in claims");
                    return Unauthorized("User ID not found in claims");
                }

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    Console.WriteLine($"UsersController: Invalid UserId format: {userIdClaim}");
                    return BadRequest("Invalid User ID format");
                }

                /* 
                 * Ищем пользователя по UserId в БД.
                 * Возвращаем List<UserSearchResponse>.
                 * Возвращаем список, так как пользователи асинхронно ищутся по частичному 
                 * совпадению введённых данных и отображаются на веб-странице
                 */
                var users = Database.DbSearchUsers(query, userId);
                Console.WriteLine($"UsersController: Found {users.Count} users for query = {query}");
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UsersController: Error searching users: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}