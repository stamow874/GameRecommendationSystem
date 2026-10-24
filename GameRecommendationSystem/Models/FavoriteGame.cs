
namespace GameRecommendationSystem.Models;

public class FavoriteGame
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int GameId { get; set; }

    public Game Game { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

