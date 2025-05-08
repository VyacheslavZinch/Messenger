using APIInterfaces.WebClient;
using Entities;
using MessengerDb;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessing.Actions
{
    public static class Database
    {
        /*
         * Метод получает данные о пользователе из БД
         * в виде сущности типа UserInfo
         */
        public static UserInfo DbGetUser(string mailOrNickName)
        {
            /*
             * в анонимной функции создаём обьект типа UserInfo,
             * в который будет сохранён результат поиска в БД.
             * в случае, если пользователь не будет найден,
             * то user вернёт null
             */
            var user = (() =>
            {
                UserInfo usr;
                using (EntityDb db = new())
                {
                    usr = db.Users
                                .FirstOrDefault(user =>
                                    user.UserEmail == mailOrNickName ||
                                    user.UserNickname == mailOrNickName);
                }
                return usr;
            });
            return user.Invoke();
        }

        public static void DbReplaceOldPassword(string password, Guid userId)
        {
            using (EntityDb db = new())
            {
                
                //Получаем текущий пароль пользователя
                var passwordInfo = db.Passwords.FirstOrDefault(pwd => pwd.UserId == userId);

                
                //Получаем соль для пароля
                var passwordSaltInfo = db.PasswordSalts.FirstOrDefault(pwd => pwd.UserId == userId);

                
                //Проверяем, вся ли нужная информация получена для выполнения транзакции в БД 
                if (passwordInfo != null && passwordSaltInfo != null)
                {
                    /*
                     * Создаём новую соль для пароля.
                     * Переводим полученную соль из массива байтов 
                     * в строку Base64 для удобного хранения в БД.
                     * Также солим пароль и сохраняем результат соления в БД
                     * в поле пароля пользователя
                     */
                    var salt = Password.GenerateSalt();
                    var saltBase64 = Password.SaltBase64(salt);
                    var passwordBase64 = Password.HashPasswordBase64(password, salt);

                    /*
                     * В полученных обьектах пароля и соли пользователя, сопоставленном с результатом из БД
                     * обновляем данные о пароле и соли
                     */
                    passwordInfo.Password = passwordBase64;
                    passwordSaltInfo.Salt = saltBase64;

                    
                    //Сохраняем изменения в БД
                     
                    db.SaveChanges();
                }
            }
        }

        public static void InsertNewUser(UserInfo newUser, string password)
        {
            /*
             * Генерируем соль для нового пользователя.
             * Солим пароль, преобразовываем соль в 
             * строку типа Base64. Солим пароль.
             */
            var salt = Password.GenerateSalt();
            var saltBase64 = Password.SaltBase64(salt);
            var hashPasswordBase64 = Password.HashPasswordBase64(password, salt);

            using (EntityDb db = new())
            {
                /*
                 * Создаём транзакцию для сохранения данных о пользователе в БД.
                 * Транзакция обеспечивает всех необходимых запросов в БД.
                 * В случае возникновения ошибки в транзакции, транзакцию можно 
                 * откатить или повторить.
                 */
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        /*
                         * Вставляем в БД в таблицы
                         * данные пользователя -> dbo.UserInfo 
                         * данные о пароле ->     dbo.UserPassword
                         * соль для пароля ->     dbo.UserPasswordSalt
                         * Данные сохраняются по очереди, так как таблицы связаны по ключу UserId
                         */
                        db.Users.Add(newUser);
                        db.SaveChanges();

                        db.Passwords.Add(new UserPassword()
                        {
                            UserId = newUser.UserId,
                            Password = hashPasswordBase64,
                        });
                        db.SaveChanges();

                        db.PasswordSalts.Add(new UserPasswordSalt()
                        {
                            UserId = newUser.UserId,
                            Salt = saltBase64,
                        });
                        db.SaveChanges();

                        
                        //Сохраняем изменения. Завершаем транзакцию
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        
                        //В случае ошибки проведения транзакции откатываем её
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        //Получаем пароль и соль пароля пользователя. Ищем по мейлу или никнейму
        public static LoginDataDb GetUserPasswordAndSalt(string mailOrNickname)
        {
            var user = Database.DbGetUser(mailOrNickname);
            LoginDataDb? response;

            if (user != null)
            {
                using (EntityDb db = new())
                {

                    var passwordInfo = db.Passwords
                            .FirstOrDefault(pwd => pwd.UserId == user.UserId);

                    var passwordSaltInfo = db.PasswordSalts
                        .FirstOrDefault(pwd => pwd.UserId == user.UserId);

                    response = new LoginDataDb()
                    {
                        Password = passwordInfo.Password,
                        Salt = passwordSaltInfo.Salt,
                    };
                }
                return response;
            }
            else
            {
                return null;
            }
        }

        //Получаем текущий список чатов пользователя
        public static List<ChatResponse> DbGetChats(Guid userId)
        {
            using (var db = new EntityDb())
            {
                var chats = db.Chats
                    .Where(c => c.UserId1 == userId || c.UserId2 == userId)
                    .Select(c => new
                    {
                        c.ChatId,
                        OtherUserId = c.UserId1 == userId ? c.UserId2 : c.UserId1,
                        OtherUser = c.UserId1 == userId ? c.User2 : c.User1,
                        LastMessage = db.Messages
                            .Where(m => m.ChatId == c.ChatId)
                            .OrderByDescending(m => m.SendDate)
                            .FirstOrDefault()
                    })
                    .ToList();

                return chats.Select(c => new ChatResponse
                {
                    ChatId = c.ChatId,
                    UserName = c.OtherUser?.UserNickname ?? "Unknown",
                    LastMessage = c.LastMessage?.ChatMessage
                }).ToList();
            }
        }

        //Проверяем, есть ли у юзера чат с пользователем
        public static bool DbChatExists(int chatId, Guid userId)
        {
            using (var db = new EntityDb())
            {
                return db.Chats
                    .Any(c => c.ChatId == chatId && (c.UserId1 == userId || c.UserId2 == userId));
            }
        }

        //Получаем список сообщений конкретного чата по ид чата
        public static List<MessageResponse> DbGetMessages(int chatId)
        {
            using (var db = new EntityDb())
            {
                return db.Messages
                    .Where(m => m.ChatId == chatId)
                    .Include(m => m.User)
                    .OrderBy(m => m.SendDate)
                    .Select(m => new MessageResponse
                    {
                        ChatMessage = m.ChatMessage,
                        UserId = m.UserId.ToString(),
                        SendDate = m.SendDate,
                        SenderNickname = m.User.UserNickname
                    })
                    .ToList();
            }
        }

        /*
         * Доблавяем новый контакт
         * Используем транзакцию для надёжности
         * при сохранении данных
         */
        public static int? DbAddContact(Guid userId, Guid contactId)
        {
            using (var db = new EntityDb())
            {
                var contact = db.Users.FirstOrDefault(u => u.UserId == contactId);
                if (contact == null)
                {
                    return null;
                }

                
                //Если у пользователя есть чат с данным контактом, то возвращаем ид чата
                 
                var existingChat = db.Chats
                    .Where(c => (c.UserId1 == userId && c.UserId2 == contactId) || (c.UserId1 == contactId && c.UserId2 == userId))
                    .Select(c => c.ChatId)
                    .FirstOrDefault();

                if (existingChat != 0)
                {
                    return existingChat;
                }


                //Создаём транзакцию для сохранения данных в БД
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        /*
                         * Создаём новые сущности-отношения 
                         * UserContacts, UserChats для записи в БД
                         * и сохраняем изменения
                         */
                        var newContact = new UserContacts
                        {
                            UserId = userId,
                            ContactId = contactId
                        };
                        db.Contacts.Add(newContact);

                        var newChat = new UserChats
                        {
                            UserId1 = userId,
                            UserId2 = contactId,
                            CreatedDate = DateTime.UtcNow
                        };
                        db.Chats.Add(newChat);

                        db.SaveChanges();
                        transaction.Commit();

                        return newChat.ChatId;
                    }
                    catch (Exception)
                    {
                        //Откатываем транзакцию в случае ошибки
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        
         //Получаем список участников чата
         
        public static List<Guid> DbGetChatParticipants(int chatId)
        {
            using (var db = new EntityDb())
            {
                
                //Получаем список участников чата по ChatId
                
                var chat = db.Chats.FirstOrDefault(c => c.ChatId == chatId);
                if (chat == null)
                {
                    return new List<Guid>();
                }

                
                //Возвращаем список UserId участников чата
                 
                return new List<Guid> { chat.UserId1, chat.UserId2 }
                    .Where(id => id != Guid.Empty)
                    .ToList();
            }
        }

        /**/
        public static (UserChatMessages message, string senderNickname)? DbAddMessage(int chatId, Guid userId, string messageContent)
        {
            using (var db = new EntityDb())
            {
                //Получаем пользователя по UserId
                 
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return null;
                }

                
                //Создаём новую сущность-отношение для записи в таблицу dbo.UserChatMessage
                var newMessage = new UserChatMessages
                {
                    ChatId = chatId,
                    UserId = userId,
                    ChatMessage = messageContent,
                    SendDate = DateTime.UtcNow
                };

                //Записываем данные в таблицу и сохраняем изменения
                db.Messages.Add(newMessage);
                db.SaveChanges();

                //Возвращаем сообщение и адресата
                return (newMessage, user.UserNickname);
            }
        }

        
        public static List<UserSearchResponse> DbSearchUsers(string query, Guid currentUserId)
        {
            using (var db = new EntityDb())
            {
                var data = db.Users
                    .Where(u => (u.UserEmail.Contains(query) || u.UserNickname.Contains(query)) && !u.UserId.Equals(currentUserId))
                    .Select(u => new UserSearchResponse
                    {
                        UserId = u.UserId.ToString(),
                        UserEmail = u.UserEmail,
                        UserNickname = u.UserNickname
                    })
                    .ToList();
                return data;
            }
        }
    }
}