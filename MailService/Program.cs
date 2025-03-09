using APIInterfaces;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MailService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
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
                var outcomingRequest = await new Mail<IncomingRequestRegistration>(requestBody).SendConfirmMail();
                return Results.Ok(outcomingRequest);
            });

            mailService.MapPost("/restoreaccess", async (HttpContext context) =>
            {
                var requestBody = await Mail<IncomingRequestRegistration>.getRequestBody<IncomingRequestRestoreAccess>(context);
                var outcomingRequest = await new Mail<IncomingRequestRestoreAccess>(requestBody).SendNewPassword();
                return Results.Ok(outcomingRequest);

            });

            app.Run();
        }
    }


    [JsonSerializable(typeof(MailSenderResult))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {

    }
}
