﻿using APIInterfaces;
using DataProcessing.Actions;
using Entities;
using Microsoft.AspNetCore.Mvc;
using MessengerDb;
using System.Net;

namespace DataProcessing.Controllers
{
    [ApiController]
    [Route("api/restore-access")]
    public class RestoreAccessController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> RestoreAccess([FromBody] APIInterfaces.RestoreAccess data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                string newPassword = Password.NewPasswordGenerator();
                var user = Database.DbGetUser(data.Mail);

                var mailServiceResponse = await Actions.RestoreAccess.RestoreAccessToAccount(
                    new IncomingRequestRestoreAccess()
                    {
                        Email = data.Mail,
                        Name = user.Username,
                        NewPassword = newPassword,
                    });

                if (mailServiceResponse.StatusCode != HttpStatusCode.OK)
                {
                    switch (mailServiceResponse.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            //LOGGER
                            return BadRequest($"{mailServiceResponse.MessageServiceAPIError}");

                        case HttpStatusCode.InternalServerError:
                            //LOGGER
                            return StatusCode(500, "The service is unavailable");

                        default:
                            //LOGGER
                            return StatusCode((int)mailServiceResponse.StatusCode, "The service is unavailable");
                    }
                }
                else
                {
                    //LOGGER
                    Database.DbReplaceOldPassword(newPassword, user.UserId);
                    return Ok();
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
