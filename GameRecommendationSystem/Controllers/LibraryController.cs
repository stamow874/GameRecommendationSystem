
using GameRecommendationSystem.Data;
using GameRecommendationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameRecommendationSystem.Controllers;

[Authorize]
public class LibraryController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public LibraryController(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        var games = await _dbContext.UserSteamGames
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.PlaytimeForeverMinutes)
            .ThenBy(x => x.Name)
            .ToListAsync();

        return View(games);
    }
}
