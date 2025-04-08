namespace Entities
{
    public class UserChatMessages
    {
        public Guid MessageId { get; set; }

        public Guid ChatId { get; set; }
        public UserChats Chat { get; set; }
        public string ChatMessage { get; set; }
        public DateTime SendDate { get; set; }
    }
}
