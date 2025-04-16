using APIInterfaces;
using DotNetEnv;
using RestSharp;
using RestSharp.Authenticators;
//using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MailService
{
    public enum MailTypes
    {
        Registration,
        RestoreAccess      
    }

    public class Mail<T> where T: class
    {
        public RestClient Client { get; set; }
        public RestRequest Request { get; set; }
        public RestResponse Response { get; set; }
        private MailSenderResult sendMessageResult;
        public string UserName { get; set; }
        public string ToMail { get; set; }
        public string NewPassword { get; set; }
        private const string RestoreAccessMessage = "Access to you account successfully restored.";
        private const string ConfirmRegistrationMessage = "Thanks for registration!";



        #region secrets

        private static Lazy<string> apiKey = new Lazy<string>(() =>
        {
            string key = Environment.GetEnvironmentVariable("MAIL_API_KEY");
            return key;
        });

        private static Lazy<string> mailDomain = new Lazy<string>(() =>
        {
            string domain = Environment.GetEnvironmentVariable("MAIL_DOMAIN");
            return domain;
        });

        private static Lazy<string> mailURL = new Lazy<string>(() =>
        {
            string uri = Environment.GetEnvironmentVariable("MAIL_URI");
            return uri;
        });

        private static Lazy<string> fromMail = new Lazy<string>(() =>
        {
            string mail = Environment.GetEnvironmentVariable("MAIL_SENDER");
            return mail;
        });

        #endregion

        public Mail(T requestBody)
        {
            Env.Load();
            
            if (typeof(T).Equals(typeof(IncomingRequestRegistration)))
            {
                this.UserName = (requestBody as IncomingRequestRegistration).Name;
                this.ToMail = (requestBody as IncomingRequestRegistration).Email;
            }
            else if (typeof(T).Equals(typeof(IncomingRequestRestoreAccess)))
            {
                this.UserName = (requestBody as IncomingRequestRestoreAccess).Name;
                this.ToMail = (requestBody as IncomingRequestRestoreAccess).Email;
                this.NewPassword = (requestBody as IncomingRequestRestoreAccess).NewPassword;
            }
            

            Request = new RestRequest($"{mailDomain.Value}/messages", Method.Post);

            Client = new RestClient(
                new RestClientOptions(mailURL.Value)
                {
                    Authenticator = new HttpBasicAuthenticator("api", apiKey.Value)
                }
            );

            sendMessageResult = new MailSenderResult()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = null,
            };

            Request.AddParameter("from", $"{fromMail.Value}");
            Request.AddParameter("to", $"{ToMail}");
        }

        public async Task<MailSenderResult> SendNewPassword()
        {
            var message = $"Hello {UserName}! This is your new password\n{NewPassword}";
#if DEBUG
            Console.WriteLine(message);
#endif

            Request.AddParameter("subject", $"{RestoreAccessMessage}");
            Request.AddParameter("text", $"{message}");

            try
            {
                sendMessageResult.MailType = MailType.RestoreAccess.ToString().ToLower();
                sendMessageResult.Message = message;
                sendMessageResult.MessageDateTime = DateTime.Now;
                Response = await Task.Run(() => Client.Execute(Request));
#if DEBUG
                Console.WriteLine(Response.Content);
#endif
                if (Response.IsSuccessful)
                {
                    sendMessageResult.StatusCode = System.Net.HttpStatusCode.OK;
                    return await Task.Run(() => sendMessageResult);
                }
                else
                {
                    sendMessageResult.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    sendMessageResult.MessageServiceAPIError = new MessageServiceAPIError
                    {
                        Message = Response.ErrorMessage,
                    };
                    return await Task.Run(() => sendMessageResult);
                }

            }
            catch (Exception err)
            {
                sendMessageResult.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                sendMessageResult.MessageServiceAPIError = new MessageServiceAPIError
                {
                    Message = err.Message,
                };
                return await Task.Run(() => sendMessageResult);
            }

        }
        public async Task<MailSenderResult> SendConfirmMail()
        {

            var message = $"Hello, {UserName}! Thanks for registration in our service";

            Request.AddParameter("subject", $"{ConfirmRegistrationMessage}");
            Request.AddParameter("text", $"{message}");

            try
            {
                sendMessageResult.MailType = MailType.Registration.ToString().ToLower();
                sendMessageResult.Message = message;
                sendMessageResult.MessageDateTime = DateTime.Now;
                Response = await Task.Run(() => Client.Execute(Request));
#if DEBUG
                Console.WriteLine(Response.Content);
#endif
                if (Response.IsSuccessful)
                {
                    sendMessageResult.StatusCode = System.Net.HttpStatusCode.OK;
                    return sendMessageResult;

                }
                else
                {
                    sendMessageResult.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    sendMessageResult.MessageServiceAPIError = new MessageServiceAPIError
                    {
                        Message = Response.ErrorMessage,
                    };
                    return sendMessageResult;
                }

            }
            catch (Exception err)
            {
                sendMessageResult.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                sendMessageResult.MessageServiceAPIError = new MessageServiceAPIError
                {
                    Message = err.Message,
                };
                return await Task.Run(() => sendMessageResult);
            }
        }

        public static async Task<T> getRequestBody<T>(HttpContext context)
            where T: class
        {
            using var reader = new StreamReader(context.Request.Body);
            string requestBodyText = await reader.ReadToEndAsync();
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            };
            T requestBody = JsonSerializer.Deserialize<T>(requestBodyText, options);

            return requestBody;
        }
    }
}

