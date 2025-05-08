using APIInterfaces.WebClient;
using DataProcessing.Actions;
using DataProcessing.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DataProcessing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;
        private readonly RedisData _redisData;

        public LoginController(JwtTokenService jwtTokenService, RedisData redisData)
        {
            _jwtTokenService = jwtTokenService;
            _redisData = redisData;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginIncomingRequest request)
        {
            try
            {
                var user = Database.DbGetUser(request.UsernameMail);
                if (user == null)
                {
                    Console.WriteLine($"LoginController: User not found for MailOrNickname = {request.UsernameMail}");
                    return NotFound("User not found");
                }

                var loginData = Database.GetUserPasswordAndSalt(request.UsernameMail);
                if (loginData == null)
                {
                    Console.WriteLine($"LoginController: Password data not found for MailOrNickname = {request.UsernameMail}");
                    return NotFound("Password data not found");
                }

                var hashedPassword = Password.HashPasswordBase64(request.Password, Convert.FromBase64String(loginData.Salt));
                if (hashedPassword != loginData.Password)
                {
                    Console.WriteLine($"LoginController: Invalid password for MailOrNickname = {request.UsernameMail}");
                    return Unauthorized("Invalid password");
                }

                var token = _jwtTokenService.GenerateToken(user.UserId.ToString(), user.UserNickname);
                await _redisData.SetTokenAsync(user.UserId.ToString(), token);

                return Ok(new LoginResponse{ UserId = user.UserId.ToString(), UserToken = token});
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoginController: Error during login: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("LoginController: UserId not found in claims during logout");
                    return Unauthorized("User ID not found in claims");
                }

                var result = await _redisData.DeleteTokenAsync(userId);
                if (!result)
                {
                    Console.WriteLine($"LoginController: Token not found for userId = {userId} during logout");
                    return NotFound("Token not found");
                }

                return Ok("Logged out successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoginController: Error during logout: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}