using DataProcessing.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace DataProcessing.Hubs
{
    [Authorize(Policy = "RedisTokenPolicy")]
    public class ChatHub : Hub
    {
        public async Task SendMessage(int chatId, string message)
        {
            try
            {
                var userIdClaim = Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    throw new HubException("User ID not found in claims");
                }

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    throw new HubException("Invalid User ID format");
                }

                if (!Database.DbChatExists(chatId, userId))
                {
                    throw new HubException("Chat not found or access denied");
                }

                var messageResult = Database.DbAddMessage(chatId, userId, message);
                if (messageResult == null)
                {
                    throw new HubException("Sender not found");
                }

                var (newMessage, senderNickname) = messageResult.Value;
                var participants = Database.DbGetChatParticipants(chatId);

                Console.WriteLine($"ChatHub: Message sent in chat {chatId} by userId = {userId}: {message}");
                foreach (var participantId in participants)
                {
                    await Clients.User(participantId.ToString()).SendAsync("ReceiveMessage", chatId, userId.ToString(), message, newMessage.SendDate, senderNickname);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ChatHub: Error sending message: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveError", $"Error sending message: {ex.Message}");
            }
        }
    }
}