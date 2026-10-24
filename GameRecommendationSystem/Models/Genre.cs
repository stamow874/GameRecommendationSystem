
using System.ComponentModel.DataAnnotations;

namespace GameRecommendationSystem.Models;

public class Genre
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }

    public ICollection<GameGenre> GameGenres { get; set; }
        = new List<GameGenre>();

    public ICollection<UserPreferredGenre> UserPreferredGenres { get; set; }
        = new List<UserPreferredGenre>();
}
