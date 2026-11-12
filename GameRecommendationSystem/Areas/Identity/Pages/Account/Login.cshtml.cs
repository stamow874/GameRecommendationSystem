
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameRecommendationSystem.Data;

namespace GameRecommendationSystem.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Моля, въведете имейл адрес.")]
        [EmailAddress(ErrorMessage = "Моля, въведете валиден имейл адрес.")]
        [Display(Name = "Имейл адрес")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Моля, въведете парола.")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Запомни ме")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        await HttpContext.SignOutAsync(
            IdentityConstants.ExternalScheme);
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        
        var user = await _userManager.FindByEmailAsync(Input.Email);

        if (user == null || string.IsNullOrEmpty(user.UserName))
        {
            ModelState.AddModelError(
                string.Empty,
                "Невалиден имейл адрес или парола.");

            return Page();
        }

      
        var result = await _signInManager.PasswordSignInAsync(
            user.UserName,
            Input.Password,
            Input.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("Потребителят влезе успешно.");

            return LocalRedirect(ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("Акаунтът е временно заключен.");

            return RedirectToPage("./Lockout");
        }

        ModelState.AddModelError(
            string.Empty,
            "Невалиден имейл адрес или парола.");

        return Page();
    }
}
