
using GameRecommendationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameRecommendationSystem.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<GameGenre> GameGenres => Set<GameGenre>();
    public DbSet<GamePlatform> GamePlatforms => Set<GamePlatform>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
    public DbSet<UserPreferredGenre> UserPreferredGenres => Set<UserPreferredGenre>();
    public DbSet<UserPreferredPlatform> UserPreferredPlatforms => Set<UserPreferredPlatform>();
    public DbSet<GameReview> GameReviews => Set<GameReview>();
    public DbSet<FavoriteGame> FavoriteGames => Set<FavoriteGame>();
    public DbSet<RecommendationHistory> RecommendationHistories
        => Set<RecommendationHistory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Една игра може да има много жанрове.
        builder.Entity<GameGenre>()
            .HasKey(x => new { x.GameId, x.GenreId });

        builder.Entity<GameGenre>()
            .HasOne(x => x.Game)
            .WithMany(x => x.GameGenres)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GameGenre>()
            .HasOne(x => x.Genre)
            .WithMany(x => x.GameGenres)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        // Една игра може да е налична на много платформи.
        builder.Entity<GamePlatform>()
            .HasKey(x => new { x.GameId, x.PlatformId });

        builder.Entity<GamePlatform>()
            .HasOne(x => x.Game)
            .WithMany(x => x.GamePlatforms)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GamePlatform>()
            .HasOne(x => x.Platform)
            .WithMany(x => x.GamePlatforms)
            .HasForeignKey(x => x.PlatformId)
            .OnDelete(DeleteBehavior.Cascade);

        // Един потребител има един профил с предпочитания.
        builder.Entity<UserPreference>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        builder.Entity<UserPreference>()
            .HasOne<IdentityUser>()
            .WithOne()
            .HasForeignKey<UserPreference>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Предпочитани жанрове.
        builder.Entity<UserPreferredGenre>()
            .HasKey(x => new { x.UserPreferenceId, x.GenreId });

        builder.Entity<UserPreferredGenre>()
            .HasOne(x => x.UserPreference)
            .WithMany(x => x.PreferredGenres)
            .HasForeignKey(x => x.UserPreferenceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserPreferredGenre>()
            .HasOne(x => x.Genre)
            .WithMany(x => x.UserPreferredGenres)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        // Предпочитани платформи.
        builder.Entity<UserPreferredPlatform>()
            .HasKey(x => new { x.UserPreferenceId, x.PlatformId });

        builder.Entity<UserPreferredPlatform>()
            .HasOne(x => x.UserPreference)
            .WithMany(x => x.PreferredPlatforms)
            .HasForeignKey(x => x.UserPreferenceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserPreferredPlatform>()
            .HasOne(x => x.Platform)
            .WithMany(x => x.UserPreferredPlatforms)
            .HasForeignKey(x => x.PlatformId)
            .OnDelete(DeleteBehavior.Cascade);

        // Потребителска оценка за игра.
        builder.Entity<GameReview>()
            .HasIndex(x => new { x.UserId, x.GameId })
            .IsUnique();

        builder.Entity<GameReview>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GameReview>()
            .HasOne(x => x.Game)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        // Любими игри — една игра не може да се добави два пъти.
        builder.Entity<FavoriteGame>()
            .HasIndex(x => new { x.UserId, x.GameId })
            .IsUnique();

        builder.Entity<FavoriteGame>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FavoriteGame>()
            .HasOne(x => x.Game)
            .WithMany(x => x.Favorites)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        // История на препоръките.
        builder.Entity<RecommendationHistory>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RecommendationHistory>()
            .HasOne(x => x.Game)
            .WithMany(x => x.RecommendationHistory)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RecommendationHistory>()
            .Property(x => x.MatchPercentage)
            .HasPrecision(5, 2);

        // Не допускаме дублирани имена на жанрове и платформи.
        builder.Entity<Genre>()
            .HasIndex(x => x.Name)
            .IsUnique();

        builder.Entity<Platform>()
            .HasIndex(x => x.Name)
            .IsUnique();
    }
}
