using System.Text.RegularExpressions;

namespace Client.Services
{
    public delegate Task<bool> checkRegistrationValue(string value);
    static class RegistrationDataValidationMethods
    {
        //this methods must call the web API
        public static async Task<bool> UsernameLengthCheck(string username)
        {
            //TODO
            return false;
        }
        public static async Task<bool> UsernameIsAcessibleCheck(string username)
        {
            //TODO
            return false;
        }
        public static async Task<bool> PasswordRequirementsCheck(string password)
        {
            //TODO
            return false;
        }
        public static async Task<bool> UsersEmailIsAcessibleCheck(string password)
        {
            //TODO
            return false;
        }

    }
}
