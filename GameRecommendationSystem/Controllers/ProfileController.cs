
using GameRecommendationSystem.Data;
using GameRecommendationSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameRecommendationSystem.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
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

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        var model = new ProfileEditViewModel
        {
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        var newUserName = model.UserName.Trim();
        var newEmail = model.Email.Trim();

        var existingUserName =
            await _userManager.FindByNameAsync(newUserName);

        if (existingUserName != null && existingUserName.Id != user.Id)
        {
            ModelState.AddModelError(
                nameof(model.UserName),
                "Това потребителско име вече е заето.");

            return View(model);
        }

        var existingEmail =
            await _userManager.FindByEmailAsync(newEmail);

        if (existingEmail != null && existingEmail.Id != user.Id)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "Този имейл адрес вече се използва.");

            return View(model);
        }

        var userNameResult =
            await _userManager.SetUserNameAsync(user, newUserName);

        if (!userNameResult.Succeeded)
        {
            foreach (var error in userNameResult.Errors)
            {
                ModelState.AddModelError(
                    nameof(model.UserName),
                    error.Description);
            }

            return View(model);
        }

        var emailResult =
            await _userManager.SetEmailAsync(user, newEmail);

        if (!emailResult.Succeeded)
        {
            foreach (var error in emailResult.Errors)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    error.Description);
            }

            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["ProfileMessage"] =
            "Профилът беше обновен успешно.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            _context.GameReviews.RemoveRange(
                _context.GameReviews.Where(x => x.UserId == user.Id));

            _context.FavoriteGames.RemoveRange(
                _context.FavoriteGames.Where(x => x.UserId == user.Id));

            _context.RecommendationHistories.RemoveRange(
                _context.RecommendationHistories.Where(x => x.UserId == user.Id));

            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();

                TempData["ProfileError"] =
                    "Акаунтът не можа да бъде изтрит. Моля, опитай отново.";

                return RedirectToAction(nameof(Index));
            }

            await transaction.CommitAsync();

            await _signInManager.SignOutAsync();

            return RedirectToPage(
                "/Account/Login",
                new { area = "Identity" });
        }
        catch
        {
            await transaction.RollbackAsync();

            TempData["ProfileError"] =
                "Възникна грешка при изтриването на акаунта.";

            return RedirectToAction(nameof(Index));
        }
    }
}
