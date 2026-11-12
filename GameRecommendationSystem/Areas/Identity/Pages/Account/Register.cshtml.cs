
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameRecommendationSystem.Data;

namespace GameRecommendationSystem.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Моля, въведете потребителско име.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Потребителското име трябва да бъде между 3 и 50 символа.")]
        [Display(Name = "Потребителско име")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Моля, въведете имейл адрес.")]
        [EmailAddress(ErrorMessage = "Моля, въведете валиден имейл адрес.")]
        [Display(Name = "Имейл адрес")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Моля, въведете парола.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Паролата трябва да бъде поне 8 символа.")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Моля, потвърдете паролата.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
        [Display(Name = "Потвърди паролата")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.UserName.Trim(),
            Email = Input.Email.Trim()
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("Успешна регистрация на нов потребител.");

            await _signInManager.SignInAsync(user, isPersistent: false);

            return LocalRedirect(ReturnUrl);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }
}
