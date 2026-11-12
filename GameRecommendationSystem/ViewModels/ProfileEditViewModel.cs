
using System.ComponentModel.DataAnnotations;

namespace GameRecommendationSystem.ViewModels;

public class ProfileEditViewModel
{
    [Required(ErrorMessage = "Потребителското име е задължително.")]
    [StringLength(50, MinimumLength = 3,
        ErrorMessage = "Потребителското име трябва да бъде между 3 и 50 символа.")]
    [Display(Name = "Потребителско име")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Имейл адресът е задължителен.")]
    [EmailAddress(ErrorMessage = "Въведете валиден имейл адрес.")]
    [Display(Name = "Имейл адрес")]
    public string Email { get; set; } = string.Empty;
}
