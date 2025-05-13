using DataProcessing.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DataProcessing.Controllers
{
    [ApiController]
    [Route("api/chats")]
    [Authorize(Policy = "RedisTokenPolicy")]
    public class ChatsController : ControllerBase
    {
        [HttpGet("get-chats")]
        public async Task<IActionResult> GetChats()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Console.WriteLine("ChatsController: UserId not found in claims");
                    return Unauthorized("User ID not found in claims");
                }

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    Console.WriteLine($"ChatsController: Invalid UserId format: {userIdClaim}");
                    return BadRequest("Invalid User ID format");
                }

                var chats = Database.DbGetChats(userId);
                Console.WriteLine($"ChatsController: Loaded {chats.Count} chats for userId = {userId}");
                return Ok(chats);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ChatsController: Error loading chats: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}