
namespace GameRecommendationSystem.Models;

public class UserSteamGame
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int AppId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? IconUrl { get; set; }

    public int PlaytimeForeverMinutes { get; set; }

    public int? PlaytimeTwoWeeksMinutes { get; set; }

    public DateTime? LastPlayedAt { get; set; }

    public DateTime LastSyncedAt { get; set; }
}
