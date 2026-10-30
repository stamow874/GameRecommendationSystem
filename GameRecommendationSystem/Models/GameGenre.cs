
namespace GameRecommendationSystem.Models;

public class GameGenre
{
    public int GameId { get; set; }

    public virtual Game Game { get; set; } = null!;

    public int GenreId { get; set; }

    public virtual Genre Genre { get; set; } = null!;
}
