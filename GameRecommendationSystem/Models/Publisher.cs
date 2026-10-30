
using System.ComponentModel.DataAnnotations;

namespace GameRecommendationSystem.Models;

public class Publisher
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(200)]
    public string? Website { get; set; }

    public ICollection<Game> Games { get; set; } = new List<Game>();
}
