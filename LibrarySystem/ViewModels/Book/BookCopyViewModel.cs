using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.web.ViewModels.Book;

public class BookCopyViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "رقم النسخة مطلوب")]
    [StringLength(30)]
    public string CopyNumber { get; set; } = string.Empty;

    public eCopyStatus Status { get; set; }

    public Guid BookId { get; set; }
}
