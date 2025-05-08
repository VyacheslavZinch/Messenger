using APIInterfaces.WebClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using WebClient.Models;

namespace WebClient.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MessagesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var accessToken = User.FindFirst("access_token")?.Value;
            Console.WriteLine($"Index - UserId: {userId}, AccessToken: {accessToken}");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Index", "Login");
            }

            return View(new MessagesViewModel
            {
                UserId = userId,
                AccessToken = accessToken
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string query)
        {
            Console.WriteLine($"SearchUsers called with query: {query}");
            var client = _httpClientFactory.CreateClient("ChatApi");
            AddAuthorizationHeader(client);
            var response = await client.GetAsync($"/api/users/search?query={query}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"SearchUsers: Failed with status code {response.StatusCode}");
                return Json(new { success = false, error = $"Failed to search users: {response.StatusCode}" });
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"SearchUsers response: {content}");
            var users = JsonSerializer.Deserialize<UserSearchResponse[]>(content);
            return Json(new { success = true, users });
        }

        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            var client = _httpClientFactory.CreateClient("ChatApi");
            AddAuthorizationHeader(client);
            var response = await client.GetAsync("/api/chats");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GetChats: Failed with status code {response.StatusCode}");
                return Json(new { success = false, error = $"Failed to load chats: {response.StatusCode}" });
            }

            var content = await response.Content.ReadAsStringAsync();
            var chats = JsonSerializer.Deserialize<ChatResponse[]>(content);
            return Json(new { success = true, chats });
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            var client = _httpClientFactory.CreateClient("ChatApi");
            AddAuthorizationHeader(client);
            var response = await client.GetAsync($"/api/messages/{chatId}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GetMessages: Failed with status code {response.StatusCode}");
                return Json(new { success = false, error = $"Failed to load messages: {response.StatusCode}" });
            }

            var content = await response.Content.ReadAsStringAsync();
            var messages = JsonSerializer.Deserialize<MessageResponse[]>(content);
            return Json(new { success = true, messages });
        }

        [HttpPost]
        public async Task<IActionResult> AddContact([FromBody] AddContactRequest request)
        {
            var client = _httpClientFactory.CreateClient("ChatApi");
            AddAuthorizationHeader(client);
            var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/contacts/add", content);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"AddContact: Failed with status code {response.StatusCode}");
                return Json(new { success = false, error = $"Failed to add contact: {response.StatusCode}" });
            }

            var chatId = await response.Content.ReadAsStringAsync();
            return Json(new { success = true, chatId });
        }

        private void AddAuthorizationHeader(HttpClient client)
        {
            var accessToken = User.FindFirst("access_token")?.Value;
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }
}