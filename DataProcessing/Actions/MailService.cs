using APIInterfaces;
using System.Net.Http;
namespace DataProcessing.Actions
{
    public static class RestoreAccess
    {
        #region secret
        public static Lazy<string> hostNameMail = new Lazy<string>(() =>
        {
            string key = Environment.GetEnvironmentVariable("MAIL_SERVICE_HOST");
            return key;
        });
        #endregion

        public static async Task<MailSenderResult> RestoreAccessToAccount(IncomingRequestRestoreAccess data)
        {
            MailSenderResult result;
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(200);
            client.BaseAddress = new Uri(hostNameMail.Value);
            try
            {
                var response = await client.PostAsJsonAsync("/restoreaccess", data);
                result = await response.Content.ReadFromJsonAsync<MailSenderResult>();

            }catch (Exception ex)
            {
                //LOGGER
                result = new MailSenderResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ServiceUnavailable
                };
            }
            return result;
        }
    }

    public static class ConfirmRegistration
    {
        public static async Task<MailSenderResult> Confirm(IncomingRequestRegistration data)
        {
            MailSenderResult result;
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(200);
            client.BaseAddress = new Uri(RestoreAccess.hostNameMail.Value);
            try
            {
                var response = await client.PostAsJsonAsync("/confirmregistration", data);
                result = await response.Content.ReadFromJsonAsync<MailSenderResult>();

            }
            catch (Exception ex)
            {
                //LOGGER
                result = new MailSenderResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ServiceUnavailable
                };
            }
            return result;
        }
    }

}
