using Entities;
using MessengerDb;
using System;
namespace DataProcessing.Actions
{
    public static class Database
    {
        public static UserInfo DbGetUser(string mail)
        {
            var user = (() =>
            {
                UserInfo usr;
                using (EntityDb db = new())
                {
                    usr = (from user in db.Users
                           where user.UserEmail == mail
                           select user)
                             .FirstOrDefault();
                }
                return usr;
            });
            return user.Invoke();
        }

        public static void DbReplaceOldPassword(string passwordBase64, string saltBase64, Guid userId)
        {

            using (EntityDb db = new())
            {
                UserPassword passwordInfo = (from pwd in db.Passwords where pwd.UserId == userId
                                             select pwd).FirstOrDefault();

                UserPasswordSalt passwordSaltInfo = (from pwd in db.PasswordSalts where pwd.UserId == userId
                                                     select pwd).FirstOrDefault();

                if (passwordInfo != null && passwordSaltInfo != null)
                {
                    passwordInfo.Password = passwordBase64;
                    passwordSaltInfo.Salt = saltBase64;

                    db.SaveChanges();
                }

            }

        }

        public static void InsertNewUser(UserInfo newUser, string password)
        {
            var salt = Password.GenerateSalt();
            var saltBase64 = Password.SaltBase64(Password.GenerateSalt());
            var hashPasswordBase64 = Password.HashPasswordBase64(password, salt);

            using (EntityDb db = new())
            {
                db.Users.Add(newUser);
                db.Passwords.Add(new UserPassword()
                {
                    UserId = newUser.UserId,
                    Password = hashPasswordBase64,
                });
                db.PasswordSalts.Add(new UserPasswordSalt()
                {
                    UserId = newUser.UserId,
                    Salt = saltBase64,
                });
                db.SaveChanges();
            }
        }
    }
}
