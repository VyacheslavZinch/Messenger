namespace Entities
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string? UserNickname { get; set; }
        public string UserEmail { get; set; }
        public string? UserPhoneNumber { get; set; }
        public DateTime UserRegistrationDate { get; set; }

        public ICollection<UserChats> Chats { get; set; }
        public UserPassword Password { get; set; }
        public UserPasswordSalt PasswordSalt { get; set; }
        public ICollection<UserContacts> Contacts { get; set; }
        public ICollection<UserContacts> IsContactOf { get; set; }
    }
}
