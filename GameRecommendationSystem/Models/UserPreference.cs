
using System.ComponentModel.DataAnnotations;

namespace GameRecommendationSystem.Models;

public class UserPreference
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int AvailableMinutesPerSession { get; set; } = 60;

    [Required, StringLength(20)]
    public string PlayStyle { get; set; } = "Any";

    [StringLength(50)]
    public string PreferredMood { get; set; } = "Any";

    [Range(0, 100)]
    public int? MinimumAgeRating { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserPreferredGenre> PreferredGenres { get; set; }
        = new List<UserPreferredGenre>();

    public ICollection<UserPreferredPlatform> PreferredPlatforms { get; set; }
        = new List<UserPreferredPlatform>();
}

