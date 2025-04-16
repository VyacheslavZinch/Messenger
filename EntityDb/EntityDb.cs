using Microsoft.EntityFrameworkCore;
using Entities;

namespace MessengerDb
{
    public class EntityDb : DbContext
    {
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<UserContacts> Contacts { get; set; }
        public DbSet<UserChats> Chats { get; set; }
        public DbSet<UserChatMessages> Messages { get; set; }
        public DbSet<UserPassword> Passwords { get; set; }
        public DbSet<UserPasswordSalt> PasswordSalts { get; set; }

        public EntityDb()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=172.24.8.14;Database=Messenger;User Id=vaclav;Password=Password12345;TrustServerCertificate=true");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // UserInfo
            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("UserInfo");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).HasMaxLength(30).IsRequired();
                entity.Property(e => e.UserNickname).HasMaxLength(10).IsRequired();
                entity.HasIndex(e => e.UserNickname).IsUnique();
                entity.Property(e => e.UserEmail).HasMaxLength(30).IsRequired();
                entity.Property(e => e.UserPhoneNumber).HasMaxLength(12);
                entity.Property(e => e.UserRegistrationDate).IsRequired();
            });

            // UserContacts
            modelBuilder.Entity<UserContacts>(entity =>
            {
                entity.ToTable("UserContacts");
                entity.HasKey(e => new { e.UserId, e.ContactId });

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Contacts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Contact)
                    .WithMany()
                    .HasForeignKey(e => e.ContactId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // UserChats
            modelBuilder.Entity<UserChats>(entity =>
            {
                entity.ToTable("UserChats");
                entity.HasKey(e => e.ChatId);

                entity.HasOne(e => e.UserInfo)
                    .WithMany(e => e.Chats)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // UserChatMessages
            modelBuilder.Entity<UserChatMessages>(entity =>
            {
                entity.ToTable("UserChatMessages");
                entity.HasKey(e => e.MessageId);
                entity.Property(e => e.ChatMessage).HasMaxLength(500).IsRequired();
                entity.Property(e => e.SendDate).IsRequired();

                entity.HasOne(e => e.Chat)
                    .WithMany(e => e.Messages)
                    .HasForeignKey(e => e.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // UserPassword
            modelBuilder.Entity<UserPassword>(entity =>
            {
                entity.ToTable("UserPassword");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Password).HasMaxLength(100).IsRequired();

                entity.HasOne(e => e.User)
                    .WithOne(e => e.Password)
                    .HasForeignKey<UserPassword>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // UserPasswordSalt
            modelBuilder.Entity<UserPasswordSalt>(entity =>
            {
                entity.ToTable("UserPasswordSalt");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Salt).HasMaxLength(100).IsRequired();

                entity.HasOne(e => e.User)
                    .WithOne(e => e.PasswordSalt)
                    .HasForeignKey<UserPasswordSalt>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }


    }
}
