
using System.ComponentModel.DataAnnotations;

namespace GameRecommendationSystem.Models;

public class RecommendationHistory
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int GameId { get; set; }

    public Game Game { get; set; } = null!;

    [Range(0, 100)]
    public decimal MatchPercentage { get; set; }

    public DateTime RecommendedAt { get; set; } = DateTime.UtcNow;
}

