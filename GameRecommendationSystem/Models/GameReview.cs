
using System.ComponentModel.DataAnnotations;

namespace GameRecommendationSystem.Models;

public class GameReview
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int GameId { get; set; }

    public Game Game { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

