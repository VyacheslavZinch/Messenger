namespace Entities
{
    public class UserPassword
    {
        public Guid UserId { get; set; }
        public string Password { get; set; }
        public UserInfo User { get; set; }
    }
}
