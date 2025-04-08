namespace Entities
{
    public class UserChats
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public UserInfo UserInfo { get; set; }
        public ICollection<UserChatMessages> Messages { get; set; }
    }
}

