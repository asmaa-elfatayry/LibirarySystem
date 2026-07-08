using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "كلمة المرور لازم تكون 6 حروف على الأقل")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
