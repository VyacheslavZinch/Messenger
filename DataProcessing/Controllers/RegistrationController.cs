using APIInterfaces;
using DataProcessing.Actions;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace DataProcessing.Controllers
{
    [ApiController]
    [Route("api/registration")]
    public class RegistrationController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UserRegistrationAccount([FromBody] Registration requestData)
        {
            Console.WriteLine(requestData);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var mailServiceResponse = await Actions.ConfirmRegistration.Confirm(
                    new IncomingRequestRegistration()
                    {
                        Name = requestData.Username,
                        Email = requestData.UserEmail,
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
                        var newUser = new UserInfo()
                        {
                            UserId = Guid.NewGuid(),
                            Username = requestData.Username,
                            UserNickname = requestData.UserNickname,
                            UserEmail = requestData.UserEmail,
                            UserPhoneNumber = requestData.UserPhoneNumber,
                            UserRegistrationDate = DateTime.Now.Date
                        };
                        Database.InsertNewUser(newUser, requestData.Password);

                        //LOGGER
                        return Ok(
                            new AuthenticationResponse()
                            {
                                UserId = newUser.UserId.ToString(),
                                UserNickName = newUser.UserNickname
                            });
                    }
                    catch (Exception ex)
                    {
                        //LOGGER
                        Console.WriteLine(ex);
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
