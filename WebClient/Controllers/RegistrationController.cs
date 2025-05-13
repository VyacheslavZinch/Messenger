using APIInterfaces;
using Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace WebClient.Controllers
{

    public class RegistrationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public RegistrationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string name, string nickname, string email, string password)
        {
            var client = _httpClientFactory.CreateClient("ChatApi");
            var apiData = new Registration()
            {
                Username = name,
                UserNickname = nickname,
                UserEmail = email,
                UserPhoneNumber = null,
                Password = password
            };
            var content = new StringContent(
                JsonSerializer.Serialize(apiData),
                Encoding.UTF8,
                "application/json"
            );
            Console.WriteLine(content.ReadAsStringAsync());
            var jsonString = JsonSerializer.Serialize(apiData);


            var response = await client.PostAsync("/api/registration", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var userData = JsonSerializer.Deserialize<AuthenticationResponse>(json);
                string userId = userData.UserId;
                string userNickname = nickname;

                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, nickname),
                new Claim("Nickname", userNickname)
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Login");
            }

            ViewBag.Error = "Ошибка регистрации. Возможно, никнейм или email уже занят.";
            return RedirectToAction("Index", "Registration");
        }
    }
}
