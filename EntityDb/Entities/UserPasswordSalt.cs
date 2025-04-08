namespace Entities
{
    public class UserPasswordSalt
    {
        public Guid UserId { get; set; }
        public string Salt { get; set; }
        public UserInfo User { get; set; }
    }
}
