using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.web.ViewModels.Account;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة")]
    public string Email { get; set; } = string.Empty;
}
