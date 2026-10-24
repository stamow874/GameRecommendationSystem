
using System.ComponentModel.DataAnnotations;

namespace GameRecommendationSystem.Models;

public class Platform
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<GamePlatform> GamePlatforms { get; set; }
        = new List<GamePlatform>();

    public ICollection<UserPreferredPlatform> UserPreferredPlatforms { get; set; }
        = new List<UserPreferredPlatform>();
}
