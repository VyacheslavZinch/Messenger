using APIInterfaces;
using DotNetEnv;
using System.Text.Json.Serialization;

namespace MailService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);

            builder.WebHost.UseUrls("http://localhost:5252");

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions
                    .TypeInfoResolverChain
                    .Insert(0, AppJsonSerializerContext.Default);
            });

            var app = builder.Build();

            var mailService = app.MapGroup("/");

            mailService.MapGet("/", () =>
            {
                return Results.Ok("Hello!");
            });

            mailService.MapPost("/confirmregistration", async (HttpContext context) =>
            {
                var requestBody = await Mail<IncomingRequestRegistration>.getRequestBody<IncomingRequestRegistration>(context);
                var outcomingResponse = await new Mail<IncomingRequestRegistration>(requestBody).SendConfirmMail();
                return Results.Ok(outcomingResponse);
            });

            mailService.MapPost("/restoreaccess", async (HttpContext context) =>
            {
                var requestBody = await Mail<IncomingRequestRestoreAccess>.getRequestBody<IncomingRequestRestoreAccess>(context);
                var outcomingResponse = await new Mail<IncomingRequestRestoreAccess>(requestBody).SendNewPassword();
                return Results.Ok(outcomingResponse);

            });

            app.Run();
        }
    }


    [JsonSerializable(typeof(MailSenderResult))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {

    }
    
}
