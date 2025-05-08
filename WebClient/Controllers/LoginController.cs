using APIInterfaces.WebClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string mailOrNickname, string password)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ChatApi");
                var requestBody = new LoginIncomingRequest
                {
                    UsernameMail = mailOrNickname,
                    Password = password
                };
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"LoginController: Login failed with status code {response.StatusCode}");
                    ViewBag.Error = "Invalid email/nickname or password.";
                    return View();
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, loginResponse.UserId),
                    new Claim("access_token", loginResponse.UserToken)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                Console.WriteLine($"LoginController: User logged in: userId = {loginResponse.UserId}");
                return RedirectToAction("Index", "Messages");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoginController: Error during login: {ex.Message}");
                ViewBag.Error = "An error occurred during login.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ChatApi");
                AddAuthorizationHeader(client);
                var response = await client.PostAsync("/api/login/logout", null);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"LoginController: Logout failed with status code {response.StatusCode}");
                    return Json(new { success = false, error = "Failed to logout" });
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Console.WriteLine("LoginController: User logged out successfully");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoginController: Error during logout: {ex.Message}");
                return Json(new { success = false, error = "An error occurred during logout" });
            }
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