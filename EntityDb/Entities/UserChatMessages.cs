namespace Entities
{
    public class UserChatMessages
    {
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public Guid UserId { get; set; }
        public string ChatMessage { get; set; }
        public DateTime SendDate { get; set; }
        public UserChats Chat { get; set; }
        public UserInfo User { get; set; }
    }
}
