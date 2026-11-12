using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class GameRecommendationSystemContext(DbContextOptions<GameRecommendationSystemContext> options) : IdentityDbContext<GameRecommendationSystem.Data.ApplicationUser>(options)
{
}
