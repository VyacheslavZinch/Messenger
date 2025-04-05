using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDb
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

    public class UserContacts
    {
        public Guid UserId { get; set; }
        public UserInfo User { get; set; }
        public Guid ContactId { get; set; }
        public UserInfo Contact { get; set; }
    }

    public class UserChats
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public UserInfo UserInfo { get; set; }
        public ICollection<UserChatMessages> Messages { get; set; }
    }

    public class UserChatMessages
    {
        public Guid MessageId { get; set; }

        public Guid ChatId { get; set; }
        public UserChats Chat { get; set; }
        public string ChatMessage { get; set; }
        public DateTime SendDate { get; set; }
    }

    public class UserPassword
    {
        public Guid UserId { get; set; }
        public string Password { get; set; }
        public UserInfo User { get; set; }
    }

    public class UserPasswordSalt
    {
        public Guid UserId { get; set; }
        public string Salt { get; set; }
        public UserInfo User { get; set; }
    }
}
