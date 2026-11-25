
using GameRecommendationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameRecommendationSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
    public DbSet<RecommendationHistory> RecommendationHistories => Set<RecommendationHistory>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<UserSteamGame> UserSteamGames => Set<UserSteamGame>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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

        builder.Entity<UserPreference>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        builder.Entity<UserPreference>()
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<UserPreference>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

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

        builder.Entity<GameReview>()
            .HasIndex(x => new { x.UserId, x.GameId })
            .IsUnique();

        builder.Entity<GameReview>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GameReview>()
            .HasOne(x => x.Game)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FavoriteGame>()
            .HasIndex(x => new { x.UserId, x.GameId })
            .IsUnique();

        builder.Entity<FavoriteGame>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FavoriteGame>()
            .HasOne(x => x.Game)
            .WithMany(x => x.Favorites)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RecommendationHistory>()
            .HasOne<ApplicationUser>()
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

        builder.Entity<Genre>()
            .HasIndex(x => x.Name)
            .IsUnique();

        builder.Entity<Platform>()
            .HasIndex(x => x.Name)
            .IsUnique();

        builder.Entity<Game>()
            .HasOne(x => x.Publisher)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.PublisherId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<UserSteamGame>()
            .HasIndex(x => new { x.UserId, x.AppId })
            .IsUnique();
 
        builder.Entity<UserSteamGame>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
