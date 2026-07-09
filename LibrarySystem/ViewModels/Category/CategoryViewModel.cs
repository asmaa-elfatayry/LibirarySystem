using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Web.ViewModels.Category;

public class CategoryViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "اسم التصنيف مطلوب")]
    [StringLength(100, ErrorMessage = "الاسم لازم يكون أقل من 100 حرف")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "الوصف لازم يكون أقل من 500 حرف")]
    public string? Description { get; set; }
}
