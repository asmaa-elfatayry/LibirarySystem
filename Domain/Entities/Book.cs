using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public class Book:BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PublishedYear { get; set; }
    public string? CoverImageUrl { get; set; }

    // Foreign Keys
    public Guid AuthorId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PublisherId { get; set; }

    // Navigation Properties (الطرف التاني من العلاقة)
    public virtual Author Author { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

}
