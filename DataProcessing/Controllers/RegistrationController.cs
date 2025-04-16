using APIInterfaces;
using DataProcessing.Actions;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DataProcessing.Controllers
{
    [ApiController]
    [Route("api/registration")]
    public class RegistrationController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UserRegistrationAccount([FromBody] Registration data)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var mailServiceResponse = await Actions.ConfirmRegistration.Confirm(
                    new IncomingRequestRegistration()
                    {
                        Name = data.Username,
                        Email = data.UserEmail,
                    });

                if (mailServiceResponse.StatusCode != HttpStatusCode.OK)
                {
                    switch (mailServiceResponse.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            //LOGGER
                            Console.WriteLine($"{mailServiceResponse.StatusCode}\n {mailServiceResponse.MessageServiceAPIError}");
                            return BadRequest($"{mailServiceResponse.MessageServiceAPIError}");

                        case HttpStatusCode.InternalServerError:
                            //LOGGER
                            Console.WriteLine($"{mailServiceResponse.StatusCode}\n{mailServiceResponse.MessageServiceAPIError}");
                            return StatusCode(500, "MailService error");

                        default:
                            //LOGGER
                            Console.WriteLine($"{mailServiceResponse.StatusCode}\n{mailServiceResponse.MessageServiceAPIError}");
                            return StatusCode((int)mailServiceResponse.StatusCode, "The service is unavailable");
                    }
                }
                else
                {
                    try
                    {
                        Database.InsertNewUser(
                            new UserInfo()
                            {
                                UserId = Guid.NewGuid(),
                                Username = data.Username,
                                UserNickname = data.UserNickname,
                                UserEmail = data.UserEmail,
                                UserPhoneNumber = data.UserPhoneNumber,
                                UserRegistrationDate = DateOnly.FromDateTime(DateTime.Now)
                            },
                            data.Password);

                        //LOGGER
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        //LOGGER
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                //LOGGER
                return StatusCode(500, "The service is unavailable");
            }
        }
    }
}
