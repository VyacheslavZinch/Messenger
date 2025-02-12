using RestSharp;
using RestSharp.Authenticators;
using APIInterfaces;
using DotNetEnv;

namespace MailService
{
#nullable disable
    public class Mail
    {
        public string UserName { get; set; }
        public string ToMail { get; set; }
        public RestClient Client { get; set; }
        public RestRequest Request { get; set; }
        public RestResponse Response { get; set; }
        private MailSenderResult SendMessageResult;
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

        public Mail(string toMail, string usersName)
        {
            Env.Load();

            this.ToMail = toMail;
            this.UserName = usersName;

            Request = new RestRequest($"{mailDomain.Value}/messages", Method.Post);
            Client = new RestClient(
                new RestClientOptions(mailURL.Value)
                {
                    Authenticator = new HttpBasicAuthenticator("api", apiKey.Value)
                }
            );
            SendMessageResult = new MailSenderResult()
            {
                Result = false,
                Message = null,
                MessageDateTime = null
            };

            Request.AddParameter("from", $"{fromMail.Value}");
            Request.AddParameter("to", $"{toMail}");
        }

        public async Task<string> SendNewPassword(string newPassword)
        {
            var message = $"Hello {UserName}! This is your new password - {newPassword}";

            Request.AddParameter("subject", $"{RestoreAccessMessage}");
            Request.AddParameter("text", $"{message}");

            try
            {

                SendMessageResult.Message = message;
                SendMessageResult.MessageDateTime = DateTime.Now.ToString();
                Response = await Task.Run(() => Client.Execute(Request));
#if DEBUG
                Console.WriteLine(Response.Content);
#endif
                if (Response.IsSuccessful)
                {
                    SendMessageResult.Result = true;
                    return await Task.Run(() => JsonDataHandler.JsonSerialize(SendMessageResult));

                }
                else
                {
                    //TODO
                    return await Task.Run(() => JsonDataHandler.JsonSerialize(SendMessageResult));    
                }

            }
            catch (Exception err)
            {
                //TODO
                return await Task.Run(() => JsonDataHandler.JsonSerialize(SendMessageResult));
            }

        }
        public async Task<string> SendConfirmMail()
        {

            var message = $"Hello, {UserName}! Thanks for registration in our service";

            Request.AddParameter("subject", $"{ConfirmRegistrationMessage}");
            Request.AddParameter("text", $"{message}");

            try
            {
                SendMessageResult.Message = message;
                SendMessageResult.MessageDateTime = DateTime.Now.ToString();
                Response = await Task.Run(() =>Client.Execute(Request));
#if DEBUG
                Console.WriteLine(Response.Content);
#endif
                if (Response.IsSuccessful)
                {
                    SendMessageResult.Result = true;
                    return JsonDataHandler.JsonSerialize(SendMessageResult);

                }
                else
                {
                    //TODO
                    return JsonDataHandler.JsonSerialize(SendMessageResult);
                }

            }
            catch (Exception err)
            {
                //TODO
                return await Task.Run(() => JsonDataHandler.JsonSerialize(SendMessageResult));
            }
        }
    }
}