using DataProcessing.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DataProcessing.Controllers
{
    [ApiController]
    [Route("api/messages")]
    [Authorize(Policy = "RedisTokenPolicy")]
    public class MessagesController : ControllerBase
    {
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Console.WriteLine("MessagesController: UserId not found in claims");
                    return Unauthorized("User ID not found in claims");
                }

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    Console.WriteLine($"MessagesController: Invalid UserId format: {userIdClaim}");
                    return BadRequest("Invalid User ID format");
                }

                if (!Database.DbChatExists(chatId, userId))
                {
                    Console.WriteLine($"MessagesController: Chat {chatId} not found or user {userId} is not a participant");
                    return NotFound("Chat not found or access denied");
                }

                var messages = Database.DbGetMessages(chatId);
                Console.WriteLine($"MessagesController: Loaded {messages.Count} messages for chatId = {chatId}");
                return Ok(messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MessagesController: Error loading messages: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}