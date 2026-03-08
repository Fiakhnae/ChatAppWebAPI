using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatMember> ChatUsers => Set<ChatMember>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserIdentity>(e =>
        {
            e.ToTable("UserIdentities");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.UserId)
                .IsRequired();

            e.HasIndex(x => x.UserId)
                .IsUnique();

            e.ComplexProperty(
                x => x.Username,
                builder =>
                {
                    builder.Property(x => x.Value)
                        .HasColumnName("Username")
                        .HasMaxLength(Username.MaxUsernameLength)
                        .IsRequired();
                });

            e.ComplexProperty(
                x => x.Email,
                builder =>
                {
                    builder.Property(x => x.Value)
                        .HasColumnName("Email")
                        .HasMaxLength(Email.MaxEmailLength)
                        .IsRequired();
                });

            e.Property(x => x.PasswordHash)
                .HasMaxLength(512)
                .IsRequired();

            e.Property(x => x.SecurityStamp)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.IdentityId)
                .IsRequired();

            e.HasIndex(x => x.IdentityId)
                .IsUnique();

            e.Property(x => x.Gender)
                .IsRequired();

            e.Property(x => x.BirthDate);

            e.Property(x => x.CreatedOnUtc)
                .IsRequired();

            e.HasOne<UserIdentity>()
                .WithOne()
                .HasForeignKey<UserIdentity>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Chat>(e =>
        {
            e.ToTable("Chats");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.Name)
                .HasMaxLength(Chat.MaxNameLength)
                .IsRequired();

            e.Property(x => x.OwnerId)
                .IsRequired();

            e.Property(x => x.CreatedOnUtc)
                .IsRequired();

            e.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasMany(x => x.Messages)
                .WithOne()
                .HasForeignKey(x => x.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(x => x.Members)
                .WithOne()
                .HasForeignKey(x => x.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Navigation(x => x.Messages)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            e.Navigation(x => x.Members)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Message>(e =>
        {
            e.ToTable("Messages");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.ChatId)
                .IsRequired();

            e.Property(x => x.UserId)
                .IsRequired();

            e.Property(x => x.Text)
                .HasMaxLength(Message.MaxTextLength)
                .IsRequired();

            e.Property(x => x.CreatedOnUtc)
                .IsRequired();

            e.Property(x => x.UpdatedOnUtc)
                .IsRequired(false);

            e.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ChatMember>(e =>
        {
            e.ToTable("ChatUsers");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.HasIndex(x => new { x.ChatId, x.UserId })
                .IsUnique();

            e.Property(x => x.ChatId)
                .IsRequired();

            e.Property(x => x.UserId)
                .IsRequired();

            e.Property(x => x.Role)
                .IsRequired();

            e.Property(x => x.JoinedOnUtc)
                .IsRequired();

            e.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}