
using Microsoft.AspNetCore.Identity;

namespace GameRecommendationSystem.Data;

public class ApplicationUser : IdentityUser
{
    
    public string? SteamId { get; set; }

    public string? SteamDisplayName { get; set; }

    public string? SteamProfileUrl { get; set; }

    public string? SteamAvatarUrl { get; set; }

    
    public DateTime? SteamConnectedAt { get; set; }

    
    public DateTime? SteamLastSyncedAt { get; set; }
}
