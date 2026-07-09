using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Web.ViewModels.Book;

public class BookViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "عنوان الكتاب مطلوب")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم ISBN مطلوب")]
    [StringLength(20)]
    public string ISBN { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(1000, 2100, ErrorMessage = "سنة النشر غير صحيحة")]
    public int? PublishedYear { get; set; }

    public string? CoverImageUrl { get; set; }

    [Required(ErrorMessage = "اختار المؤلف")]
    public Guid AuthorId { get; set; }

    [Required(ErrorMessage = "اختار التصنيف")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "اختار الناشر")]
    public Guid PublisherId { get; set; }



    // الملف الجديد اللي المستخدم هيرفعه - مش بيتخزن في الداتابيز أبدًا
    public IFormFile? CoverImageFile { get; set; }

    // للعرض بس - مش بتتخزن، هي بس بتملى الـ Dropdowns في الفورم
    public IEnumerable<SelectListItem> Authors { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Publishers { get; set; } = new List<SelectListItem>();

    // للعرض في الـ Index بس (بعد الـ Include)
    public string? AuthorName { get; set; }
    public string? CategoryName { get; set; }
    public string? PublisherName { get; set; }
}
