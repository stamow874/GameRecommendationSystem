
namespace GameRecommendationSystem.Models;

public class UserPreferredPlatform
{
    public int UserPreferenceId { get; set; }

    public UserPreference UserPreference { get; set; } = null!;

    public int PlatformId { get; set; }

    public Platform Platform { get; set; } = null!;
}

