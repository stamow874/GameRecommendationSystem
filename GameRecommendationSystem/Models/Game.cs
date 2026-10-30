
using System.ComponentModel.DataAnnotations;

namespace GameRecommendationSystem.Models;

public class Game
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Developer { get; set; }

    public int? PublisherId { get; set; }

    public Publisher? Publisher { get; set; }

    public DateTime? ReleaseDate { get; set; }

    [Range(0, 1000)]
    public int EstimatedPlaytimeHours { get; set; }

    [Range(0, 100)]
    public int? AgeRating { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<GameGenre> GameGenres { get; set; }
        = new List<GameGenre>();

    public ICollection<GamePlatform> GamePlatforms { get; set; }
        = new List<GamePlatform>();

    public ICollection<GameReview> Reviews { get; set; }
        = new List<GameReview>();

    public ICollection<FavoriteGame> Favorites { get; set; }
        = new List<FavoriteGame>();

    public ICollection<RecommendationHistory> RecommendationHistory { get; set; }
        = new List<RecommendationHistory>();
}
