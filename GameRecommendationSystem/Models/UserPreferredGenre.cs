
namespace GameRecommendationSystem.Models;

public class UserPreferredGenre
{
    public int UserPreferenceId { get; set; }

    public UserPreference UserPreference { get; set; } = null!;

    public int GenreId { get; set; }

    public Genre Genre { get; set; } = null!;
}
