using APIInterfaces.WebClient;
using DataProcessing.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DataProcessing.Controllers
{
    [ApiController]
    [Route("api/logout")]
    public class LogoutController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> UserLogout([FromBody] Logout data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var redis = new RedisData(ConnectionMultiplexer.Connect(RedisData.RedisConfig));
                if (await redis.DeleteTokenAsync(data.UserId))
                {
                    return Ok();
                }
                else
                {
                    //LOGGER
                    return Ok();
                }
          
            }
            catch (Exception ex)
            {
                //LOGER
                return Ok();
            }
        }
    }
}
