using APIInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace WebClient.Controllers
{
    public class RestoreAccessController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RestoreAccessController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestoreAccess(string email)
        {
            var client = _httpClientFactory.CreateClient("ChatApi");
            var apiData = new RestoreAccess()
            {
                Mail = email
            };
            var content = new StringContent(
                JsonSerializer.Serialize(apiData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("/api/restore-access", content);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Success = "Письмо с новым паролем отправлено на ваш email.";
                return View();
            }

            ViewBag.Error = "Email не найден.";
            return View();
        }

    }
}
