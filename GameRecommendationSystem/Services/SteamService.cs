
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameRecommendationSystem.Models;

namespace GameRecommendationSystem.Services;

public class SteamService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public SteamService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<UserSteamGame>?> GetOwnedGamesAsync(string steamId)
    {
        var apiKey = _configuration["Steam:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Steam API ключът не е конфигуриран.");
        }

        if (string.IsNullOrWhiteSpace(steamId))
        {
            return null;
        }

        var url =
            "https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/" +
            $"?key={Uri.EscapeDataString(apiKey)}" +
            $"&steamid={Uri.EscapeDataString(steamId)}" +
            "&include_appinfo=1" +
            "&include_played_free_games=1";

        try
        {
            var result = await _httpClient.GetFromJsonAsync<SteamApiResult>(
                url,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var response = result?.Response;

            if (response?.GameCount == null)
            {
                return null;
            }

            if (response.GameCount == 0)
            {
                return new List<UserSteamGame>();
            }

            if (response.Games == null ||
                response.Games.Count != response.GameCount)
            {
                return null;
            }

            var syncedAt = DateTime.UtcNow;

            return response.Games.Select(game => new UserSteamGame
            {
                AppId = game.AppId,
                Name = game.Name ?? $"Steam App {game.AppId}",
                IconUrl = string.IsNullOrWhiteSpace(game.ImgIconUrl)
                    ? null
                    : $"https://media.steampowered.com/steamcommunity/public/images/apps/{game.AppId}/{game.ImgIconUrl}.jpg",
                PlaytimeForeverMinutes = game.PlaytimeForever,
                PlaytimeTwoWeeksMinutes = game.PlaytimeTwoWeeks,
                LastPlayedAt = game.LastPlayed > 0
                    ? DateTimeOffset.FromUnixTimeSeconds(game.LastPlayed).UtcDateTime
                    : null,
                LastSyncedAt = syncedAt
            }).ToList();
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    private sealed class SteamApiResult
    {
        [JsonPropertyName("response")]
        public SteamApiResponse? Response { get; set; }
    }

    private sealed class SteamApiResponse
    {
        [JsonPropertyName("game_count")]
        public int? GameCount { get; set; }

        [JsonPropertyName("games")]
        public List<SteamApiGame>? Games { get; set; }
    }

    private sealed class SteamApiGame
    {
        [JsonPropertyName("appid")]
        public int AppId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("img_icon_url")]
        public string? ImgIconUrl { get; set; }

        [JsonPropertyName("playtime_forever")]
        public int PlaytimeForever { get; set; }

        [JsonPropertyName("playtime_2weeks")]
        public int? PlaytimeTwoWeeks { get; set; }

        [JsonPropertyName("rtime_last_played")]
        public long LastPlayed { get; set; }
    }
}
