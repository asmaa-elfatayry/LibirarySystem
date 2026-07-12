using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.web.ViewModels.Loan;

public class LoanViewModel
{
    [Required(ErrorMessage = "اختار العضو")]
    public Guid MemberId { get; set; }

    [Required(ErrorMessage = "اختار الكتاب")]
    public Guid BookId { get; set; }

    public IEnumerable<SelectListItem> Members { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Books { get; set; } = new List<SelectListItem>();
}
