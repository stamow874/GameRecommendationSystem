
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using GameRecommendationSystem.Data;
using GameRecommendationSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace GameRecommendationSystem.Controllers;

[Authorize]
public class SteamController : Controller
{
    private const string SteamOpenIdUrl = "https://steamcommunity.com/openid/login";
    private const string StateCookieName = "Hrumka.Steam.OpenId.State";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApplicationDbContext _dbContext;
    private readonly SteamService _steamService;

    public SteamController(
        UserManager<ApplicationUser> userManager,
        IHttpClientFactory httpClientFactory,
        ApplicationDbContext dbContext,
        SteamService steamService)
    {
        _userManager = userManager;
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _steamService = steamService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        if (string.IsNullOrWhiteSpace(user.SteamId))
        {
            TempData["SteamError"] = "Първо трябва да свържете Steam профил.";
            return RedirectToAction(nameof(Index));
        }

        var steamGames = await _steamService.GetOwnedGamesAsync(user.SteamId);

        if (steamGames == null)
        {
            TempData["SteamError"] = "Не успяхме да заредим Steam библиотеката. Съществуващите игри са запазени.";
            return RedirectToAction(nameof(Index));
        }

        var existingGames = await _dbContext.UserSteamGames
            .Where(x => x.UserId == user.Id)
            .ToListAsync();

        var existingGamesByAppId = existingGames.ToDictionary(x => x.AppId);
        var receivedAppIds = steamGames.Select(x => x.AppId).ToHashSet();

        foreach (var steamGame in steamGames)
        {
            if (existingGamesByAppId.TryGetValue(steamGame.AppId, out var existingGame))
            {
                existingGame.Name = steamGame.Name;
                existingGame.IconUrl = steamGame.IconUrl;
                existingGame.PlaytimeForeverMinutes = steamGame.PlaytimeForeverMinutes;
                existingGame.PlaytimeTwoWeeksMinutes = steamGame.PlaytimeTwoWeeksMinutes;
                existingGame.LastPlayedAt = steamGame.LastPlayedAt;
                existingGame.LastSyncedAt = steamGame.LastSyncedAt;
            }
            else
            {
                steamGame.UserId = user.Id;
                _dbContext.UserSteamGames.Add(steamGame);
            }
        }

        var removedGames = existingGames
            .Where(x => !receivedAppIds.Contains(x.AppId))
            .ToList();

        _dbContext.UserSteamGames.RemoveRange(removedGames);

        user.SteamLastSyncedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        TempData["SteamSuccess"] = $"Синхронизацията приключи успешно. Заредени игри: {steamGames.Count}.";

        return RedirectToAction(nameof(Index));
    }

    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Disconnect()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        var games = await _dbContext.UserSteamGames
            .Where(x => x.UserId == user.Id)
            .ToListAsync();

        _dbContext.UserSteamGames.RemoveRange(games);

        user.SteamId = null;
        user.SteamDisplayName = null;
        user.SteamProfileUrl = null;
        user.SteamAvatarUrl = null;
        user.SteamConnectedAt = null;
        user.SteamLastSyncedAt = null;

        await _dbContext.SaveChangesAsync();

        TempData["SteamSuccess"] = "Steam профилът е изключен успешно. Можеш да свържеш друг профил.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Connect(bool popup = false)
    {
        var callbackBaseUrl = Url.Action(
            nameof(Callback),
            "Steam",
            new { popup },
            Request.Scheme);

        if (string.IsNullOrWhiteSpace(callbackBaseUrl))
        {
            TempData["SteamError"] = "Не успяхме да подготвим свързването със Steam.";
            return RedirectToAction(nameof(Index));
        }

        var state = WebEncoders.Base64UrlEncode(
            RandomNumberGenerator.GetBytes(32));

        Response.Cookies.Append(
            StateCookieName,
            state,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                IsEssential = true,
                MaxAge = TimeSpan.FromMinutes(10),
                Path = "/"
            });

        var callbackUrl = QueryHelpers.AddQueryString(
            callbackBaseUrl,
            "state",
            state);

        var realm = $"{Request.Scheme}://{Request.Host}/";

        var parameters = new Dictionary<string, string?>
        {
            ["openid.ns"] = "http://specs.openid.net/auth/2.0",
            ["openid.mode"] = "checkid_setup",
            ["openid.return_to"] = callbackUrl,
            ["openid.realm"] = realm,
            ["openid.identity"] = "http://specs.openid.net/auth/2.0/identifier_select",
            ["openid.claimed_id"] = "http://specs.openid.net/auth/2.0/identifier_select"
        };

        return Redirect(QueryHelpers.AddQueryString(SteamOpenIdUrl, parameters));
    }

    [HttpGet]
    public async Task<IActionResult> Callback(bool popup = false)
    {
        var state = Request.Query["state"].ToString();
        var savedState = Request.Cookies[StateCookieName];

        Response.Cookies.Delete(
            StateCookieName,
            new CookieOptions
            {
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

        if (string.IsNullOrWhiteSpace(state) ||
            string.IsNullOrWhiteSpace(savedState) ||
            !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(state),
                Encoding.UTF8.GetBytes(savedState)))
        {
            return FinishConnection(false, "Заявката за свързване е невалидна или е изтекла.", popup);
        }

        var callbackBaseUrl = Url.Action(
            nameof(Callback),
            "Steam",
            new { popup },
            Request.Scheme);

        if (string.IsNullOrWhiteSpace(callbackBaseUrl))
        {
            return FinishConnection(false, "Не успяхме да проверим адреса за връщане.", popup);
        }

        var expectedReturnUrl = QueryHelpers.AddQueryString(
            callbackBaseUrl,
            "state",
            state);

        var returnedUrl = Request.Query["openid.return_to"].ToString();
        var claimedId = Request.Query["openid.claimed_id"].ToString();
        var identity = Request.Query["openid.identity"].ToString();
        var endpoint = Request.Query["openid.op_endpoint"].ToString();
        var mode = Request.Query["openid.mode"].ToString();
        var openIdNamespace = Request.Query["openid.ns"].ToString();
        var signedFields = Request.Query["openid.signed"].ToString();

        var signedFieldList = signedFields.Split(',');

        if (mode != "id_res" ||
            openIdNamespace != "http://specs.openid.net/auth/2.0" ||
            returnedUrl != expectedReturnUrl ||
            endpoint != SteamOpenIdUrl ||
            claimedId != identity ||
            !signedFieldList.Contains("claimed_id") ||
            !signedFieldList.Contains("identity") ||
            !signedFieldList.Contains("return_to"))
        {
            return FinishConnection(false, "Steam не успя да потвърди профила.", popup);
        }

        if (!Uri.TryCreate(claimedId, UriKind.Absolute, out var claimedUri))
        {
            return FinishConnection(false, "Получен е невалиден Steam профил.", popup);
        }

        var pathParts = claimedUri.AbsolutePath.Split(
            '/',
            StringSplitOptions.RemoveEmptyEntries);

        if (!string.Equals(claimedUri.Host, "steamcommunity.com", StringComparison.OrdinalIgnoreCase) ||
            pathParts.Length != 3 ||
            pathParts[0] != "openid" ||
            pathParts[1] != "id")
        {
            return FinishConnection(false, "Получен е невалиден Steam профил.", popup);
        }

        var steamId = pathParts[2];

        if (!Regex.IsMatch(steamId, @"^\d{17}$"))
        {
            return FinishConnection(false, "Получен е невалиден Steam ID.", popup);
        }

        var parameters = Request.Query
            .Where(item => item.Key.StartsWith("openid.", StringComparison.Ordinal))
            .ToDictionary(
                item => item.Key,
                item => item.Value.ToString());

        parameters["openid.mode"] = "check_authentication";

        using var content = new FormUrlEncodedContent(parameters);

        var client = _httpClientFactory.CreateClient();

        using var response = await client.PostAsync(SteamOpenIdUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            return FinishConnection(false, "Проверката със Steam беше неуспешна.", popup);
        }

        var verificationResult = await response.Content.ReadAsStringAsync();

        var isValid = verificationResult
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Any(line => line.Trim() == "is_valid:true");

        if (!isValid)
        {
            return FinishConnection(false, "Steam не потвърди самоличността на профила.", popup);
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        var anotherUserHasThisSteamId = await _dbContext.Users
            .AnyAsync(x => x.Id != user.Id && x.SteamId == steamId);

        if (anotherUserHasThisSteamId)
        {
            return FinishConnection(false, "Този Steam профил вече е свързан с друг акаунт в Хрумка.", popup);
        }

        user.SteamId = steamId;
        user.SteamProfileUrl = $"https://steamcommunity.com/profiles/{steamId}";
        user.SteamConnectedAt = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return FinishConnection(false, "Профилът беше потвърден, но не успяхме да запазим връзката.", popup);
        }

        return FinishConnection(true, "Steam профилът е свързан успешно.", popup);
    }

    private IActionResult FinishConnection(bool success, string message, bool popup)
    {
        if (!popup)
        {
            if (success)
            {
                TempData["SteamSuccess"] = message;
            }
            else
            {
                TempData["SteamError"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        var successJson = JsonSerializer.Serialize(success);
        var messageJson = JsonSerializer.Serialize(message);
        var originJson = JsonSerializer.Serialize($"{Request.Scheme}://{Request.Host}");

        var html = $$"""
            <!DOCTYPE html>
            <html lang="bg">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>Хрумка - Steam</title>
            </head>
            <body>
                <p>{{message}}</p>
                <script>
                    if (window.opener) {
                        window.opener.postMessage({
                            type: "hrumka-steam",
                            success: {{successJson}},
                            message: {{messageJson}}
                        }, {{originJson}});
                        window.close();
                    }
                </script>
            </body>
            </html>
            """;

        return Content(html, "text/html", Encoding.UTF8);
    }
}
