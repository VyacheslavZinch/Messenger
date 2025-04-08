namespace Entities
{
    public class UserContacts
    {
        public Guid UserId { get; set; }
        public UserInfo User { get; set; }
        public Guid ContactId { get; set; }
        public UserInfo Contact { get; set; }
    }
}
