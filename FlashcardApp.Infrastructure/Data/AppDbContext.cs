using FlashcardApp.Core.Auth.Models;
using FlashcardApp.Core.Models; // Utwórz folder Models i dodaj klasy poniżej
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApp.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Friend> Friends { get; set; }
    public DbSet<FlashcardCategory> FlashcardCategories { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }
    public DbSet<Lesson> Lessons { get; set; } 
    public DbSet<UserProgress> UserProgress { get; set; }
    public DbSet<Lobby> Lobbies { get; set; }
    public DbSet<LobbyParticipant> LobbyParticipants { get; set; }
    public DbSet<Score> Scores { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfiguracja niestandardowych pól w AspNetUsers
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Nickname).HasMaxLength(50).IsRequired();
            entity.Property(u => u.Points).HasDefaultValue(0);
            entity.Property(u => u.StreakCount).HasDefaultValue(0);
            entity.Property(u => u.ConsecutiveDays).HasDefaultValue(0);
            entity.Property(u => u.SubscriptionType).HasMaxLength(50).HasDefaultValue("Free");
            entity.Property(u => u.SubscriptionEndDate);
            entity.Property(u => u.EmailVerified).HasDefaultValue(false);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Konfiguracja tabel z schematu
        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.HasIndex(f => new { f.UserId1, f.UserId2 }).IsUnique();
            entity.Property(f => f.Status).HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(f => f.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne<User>().WithMany().HasForeignKey(f => f.UserId1).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<User>().WithMany().HasForeignKey(f => f.UserId2).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FlashcardCategory>(entity =>
        {
            entity.HasKey(fc => fc.Id);
            entity.Property(fc => fc.Name).HasMaxLength(100).IsRequired();
            entity.Property(fc => fc.IsPublic).HasDefaultValue(false);
            entity.Property(fc => fc.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne<User>().WithMany().HasForeignKey(fc => fc.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Flashcard>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Word).HasMaxLength(100).IsRequired();
            entity.Property(f => f.Translation).HasMaxLength(100).IsRequired();
            entity.Property(f => f.ExampleUsage);
            entity.Property(f => f.Tags).HasColumnType("varchar[]");
            entity.Property(f => f.Difficulty).HasMaxLength(20).HasDefaultValue("Medium");
            entity.Property(f => f.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne<Lesson>().WithMany().HasForeignKey(f => f.LessonId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserProgress>(entity =>
        {
            entity.HasKey(up => up.Id);
            entity.HasIndex(up => new { up.UserId, up.FlashcardId }).IsUnique();
            entity.Property(up => up.Status).HasMaxLength(20).HasDefaultValue("NotLearned");
            entity.Property(up => up.CorrectAnswers).HasDefaultValue(0);
            entity.Property(up => up.WrongAnswers).HasDefaultValue(0);
            entity.Property(up => up.ReviewInterval).HasDefaultValue(1);
            entity.Property(up => up.EaseFactor).HasDefaultValue(2.5m);
            entity.HasOne<User>().WithMany().HasForeignKey(up => up.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Flashcard>().WithMany().HasForeignKey(up => up.FlashcardId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Lobby>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.GameMode).HasMaxLength(20).HasDefaultValue("Points");
            entity.Property(l => l.Status).HasMaxLength(20).HasDefaultValue("Open");
            entity.Property(l => l.MaxPlayers).HasDefaultValue(4);
            entity.Property(l => l.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne<User>().WithMany().HasForeignKey(l => l.CreatorId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<FlashcardCategory>().WithMany().HasForeignKey(l => l.FlashcardCategoryId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<LobbyParticipant>(entity =>
        {
            entity.HasKey(lp => lp.Id);
            entity.HasIndex(lp => new { lp.LobbyId, lp.UserId }).IsUnique();
            entity.Property(lp => lp.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne<Lobby>().WithMany().HasForeignKey(lp => lp.LobbyId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<User>().WithMany().HasForeignKey(lp => lp.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => new { s.LobbyId, s.UserId }).IsUnique();
            entity.Property(s => s.Points).IsRequired();
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne<Lobby>().WithMany().HasForeignKey(s => s.LobbyId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<User>().WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.NotificationType).HasMaxLength(50).IsRequired();
            entity.Property(n => n.Title).HasMaxLength(100).IsRequired();
            entity.Property(n => n.Message).IsRequired();
            entity.Property(n => n.IsRead).HasDefaultValue(false);
            entity.Property(n => n.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne<User>().WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}