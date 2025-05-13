using DataProcessing.Actions;
using DataProcessing.Controllers;
using DataProcessing.Hubs;
using DataProcessing.Models;
using DotNetEnv;
using MessengerDb;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Text;

namespace DataProcessing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("https://localhost:5254");

            // Настройка аутентификации (JWT + Cookies)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "Messenger", // Совпадает с appsettings.json
                    ValidAudience = "MessengerClient", // Совпадает с appsettings.json
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("kXO3fcBG7hba8/tTGhFgjCHtRQcRDybmuPSXxESW2PI=")) // Совпадает с appsettings.json
                };
            })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            // Регистрация Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(RedisData.RedisConfig));
            builder.Services.AddScoped<RedisData>();
            builder.Services.AddScoped<JwtTokenService>();

            // Регистрация JwtSettings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            // Инициализация базы данных
            new EntityDb();

            // Добавляем IHttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            // Настраиваем авторизацию
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RedisTokenPolicy", policy =>
                {
                    policy.AddRequirements(new RedisTokenRequirement());
                });
            });

            // Регистрируем обработчик авторизации как Scoped
            builder.Services.AddScoped<IAuthorizationHandler, RedisTokenAuthorizationHandler>();

            // Добавляем SignalR, контроллеры, Swagger и CORS
            builder.Services.AddSignalR();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMVC", policy =>
                {
                    policy.WithOrigins("https://localhost:5258")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            // Конфигурация пайплайна HTTP-запросов
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowMVC");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<ChatHub>("/chathub");
            app.MapControllers();

            app.Run();
        }
    }
}