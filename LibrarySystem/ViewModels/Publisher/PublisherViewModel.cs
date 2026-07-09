using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.web.ViewModels.Publisher;

public class PublisherViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "اسم الناشر مطلوب")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(100, ErrorMessage = "بيانات التواصل طويلة أكتر من اللازم")]
    public string? ContactInfo { get; set; }
}
