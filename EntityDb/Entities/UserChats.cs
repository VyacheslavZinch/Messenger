namespace Entities
{
    public class UserChats
    {
        public int ChatId { get; set; }
        public Guid UserId1 { get; set; }
        public Guid UserId2 { get; set; }
        public string? ChatName { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserInfo User1 { get; set; }
        public UserInfo User2 { get; set; }
        public ICollection<UserChatMessages> Messages { get; set; }
    }

}

