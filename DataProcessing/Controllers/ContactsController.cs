using APIInterfaces.WebClient;
using DataProcessing.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DataProcessing.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    [Authorize(Policy = "RedisTokenPolicy")]
    public class ContactsController : ControllerBase
    {
        [HttpPost("add")]
        public async Task<IActionResult> AddContact([FromBody] AddContactRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Console.WriteLine("ContactsController: UserId not found in claims");
                    return Unauthorized("User ID not found in claims");
                }

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    Console.WriteLine($"ContactsController: Invalid UserId format: {userIdClaim}");
                    return BadRequest("Invalid User ID format");
                }

                if (string.IsNullOrEmpty(request.ContactId))
                {
                    Console.WriteLine("ContactsController: ContactId is empty");
                    return BadRequest("Contact ID cannot be empty");
                }

                if (!Guid.TryParse(request.ContactId, out var contactId))
                {
                    Console.WriteLine($"ContactsController: Invalid ContactId format: {request.ContactId}");
                    return BadRequest("Invalid Contact ID format");
                }

                if (userId == contactId)
                {
                    Console.WriteLine("ContactsController: Cannot add self as contact");
                    return BadRequest("Cannot add yourself as a contact");
                }

                var chatId = Database.DbAddContact(userId, contactId);
                if (chatId == null)
                {
                    Console.WriteLine($"ContactsController: Contact not found for contactId = {contactId}");
                    return NotFound("Contact not found");
                }

                Console.WriteLine($"ContactsController: Created new chat {chatId} between userId = {userId} and contactId = {contactId}");
                return Ok(chatId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ContactsController: Error adding contact: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}