using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.web.ViewModels.Author;

public class AuthorViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "اسم المؤلف مطلوب")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "النبذة لازم تكون أقل من 1000 حرف")]
    public string? Bio { get; set; }

    [StringLength(50)]
    public string? Nationality { get; set; }
}
