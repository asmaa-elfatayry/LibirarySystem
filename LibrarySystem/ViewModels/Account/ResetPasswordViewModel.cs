using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.web.ViewModels.Account;

public class ResetPasswordViewModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "كلمة المرور لازم تكون 6 حروف على الأقل")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "كلمتا المرور غير متطابقتين")]
    public string ConfirmPassword { get; set; } = string.Empty;
}