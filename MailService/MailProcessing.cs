using RestSharp;
using RestSharp.Authenticators;
using APIInterfaces;
using DotNetEnv;
namespace MailService
{
#nullable disable
    public class Mail
    {
        private string usersName { get; set; }
        private string toMail { get; set; }
        private RestClient client { get; set; } 
        private RestRequest request { get; set; }
        private RestResponse response { get; set; }
        private MailSenderResult sendMessageResult = new MailSenderResult()
        {
            result = false,
            message = null,
            message_date_time = null
        };

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

            this.toMail = toMail;
            this.usersName = usersName;

            request = new RestRequest($"{mailDomain.Value}/messages", Method.Post);
            client = new RestClient(
                new RestClientOptions(mailURL.Value)
                {
                    Authenticator = new HttpBasicAuthenticator("api", apiKey.Value)
                }
            );
            
            request.AddParameter("from", $"{fromMail.Value}");
            request.AddParameter("to", $"{toMail}");
        }

        public async Task<string> SendNewPassword(string newPassword)
        {
            var subject = "Access to you account successfully restored.";
            var message = $"Hello {usersName}! This is your new password - {newPassword}";

            request.AddParameter("subject", $"{subject}");
            request.AddParameter("text", $"{message}");

            try
            {
  
                sendMessageResult.message = message;
                sendMessageResult.message_date_time = DateTime.Now.ToString();
                response = await Task.Run(() => client.Execute(request));
#if DEBUG
                Console.WriteLine(response.Content);
#endif
                if (response.IsSuccessful)
                {
                    sendMessageResult.result = true;
                    return await Task.Run(() => JsonDataHandler.JsonSerialize(sendMessageResult));

                }
                else
                {
                    //TODO
                    return await Task.Run(() => JsonDataHandler.JsonSerialize(sendMessageResult));    
                }

            }
            catch (Exception err)
            {
                //TODO
                return await Task.Run(() => JsonDataHandler.JsonSerialize(sendMessageResult));
            }

        }
        public async Task<string> SendConfirmMail()
        {
            var subject = "Thanks for registration!";
            var message = $"Hello, {usersName}! Thanks for registration in our service";

            request.AddParameter("subject", $"{subject}");
            request.AddParameter("text", $"{message}");

            try
            {
                sendMessageResult.message = message;
                sendMessageResult.message_date_time = DateTime.Now.ToString();
                response = await Task.Run(() =>client.Execute(request));
#if DEBUG
                Console.WriteLine(response.Content);
#endif
                if (response.IsSuccessful)
                {
                    sendMessageResult.result = true;
                    return JsonDataHandler.JsonSerialize(sendMessageResult);

                }
                else
                {
                    //TODO
                    return JsonDataHandler.JsonSerialize(sendMessageResult);
                }

            }
            catch (Exception err)
            {
                //TODO
                return await Task.Run(() => JsonDataHandler.JsonSerialize(sendMessageResult));
            }
        }
    }
}