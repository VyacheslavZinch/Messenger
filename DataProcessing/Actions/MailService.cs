using APIInterfaces;
using System.Net.Http;
namespace DataProcessing.Actions
{
    public static class RestoreAccess
    {
        #region secret

        //лениво инициализируем статическое поле при первом обращении к нему
        public static Lazy<string> hostNameMail = new Lazy<string>(() =>
        {
            string key = Environment.GetEnvironmentVariable("MAIL_SERVICE_HOST");
            return key;
        });
        #endregion

        public static async Task<MailSenderResult> RestoreAccessToAccount(IncomingRequestRestoreAccess data)
        {
            //создаём обьект http-запроса
            MailSenderResult result;
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(200);
            client.BaseAddress = new Uri(hostNameMail.Value);
            try
            {
                /*
                 * обращаемся к апи почтового сервиса на указанный ендпоинт
                 * в теле http-запроса передаём данные в виде сериализованного обьекта
                 * типа IncomingRequestRestoreAccess в Json
                 */
                var response = await client.PostAsJsonAsync("/restoreaccess", data);

                /*
                 * получаем результат запроса
                 * пробуем десериализовать его в MailSenderResult
                 */
                result = await response.Content.ReadFromJsonAsync<MailSenderResult>();

            }catch (Exception ex)
            {
                //LOGGER

                /*
                 * при возникновении ошибки передаём статус-код некорректного запроса
                 * данный код будет обрабатываться далее в цепочке обработки результатов
                 * вызова метода
                 */
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
            //создаём обьект http-запроса
            MailSenderResult result;
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(200);
            client.BaseAddress = new Uri(RestoreAccess.hostNameMail.Value);
            try
            {
                /*
                 * обращаемся к апи почтового сервиса на указанный ендпоинт
                 * в теле http-запроса передаём данные в виде сериализованного обьекта
                 * типа IncomingRequestRegistration в Json
                 */
                var response = await client.PostAsJsonAsync("/confirmregistration", data);

                /*
                 * получаем результат запроса
                 * пробуем десериализовать его в MailSenderResult
                 */
                result = await response.Content.ReadFromJsonAsync<MailSenderResult>();

            }
            catch (Exception ex)
            {
                //LOGGER
                result = new MailSenderResult()
                {
                    /*
                     * при возникновении ошибки передаём статус-код некорректного запроса
                     * данный код будет обрабатываться далее в цепочке обработки результатов
                     * вызова метода
                     */
                   
                    StatusCode = System.Net.HttpStatusCode.ServiceUnavailable
                };
            }
            return result;
        }
    }

}
