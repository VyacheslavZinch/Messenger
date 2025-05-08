using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography.X509Certificates;

namespace WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("https://localhost:5258");

            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login"; // Страница логина
                    options.AccessDeniedPath = "/AccessDenied"; // Страница для отказа в доступе
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(720); // Время жизни куки (30 минут)
                    options.SlidingExpiration = true; // Продлевать время жизни куки при активности
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.HttpOnly = true; // Защита от XSS
                    options.Cookie.SameSite = SameSiteMode.Lax; // Защита от CSRF
                    options.Events.OnRedirectToLogin = context =>
                    {
                        // При истечении куки перенаправляем на страницу логина
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    };
                });

            builder.Services.AddHttpClient("ChatApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5254");
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/");

            app.Run();
        }

        public static Lazy<string> apiHost = new Lazy<string>(() =>
        {
            string key = Environment.GetEnvironmentVariable("PROCESSING_API_HOST");
            return key;
        });
    }
}
