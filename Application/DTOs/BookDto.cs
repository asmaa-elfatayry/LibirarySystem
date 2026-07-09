using System;

namespace Application.DTOs;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PublisherId { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Description { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public int? PublishedYear { get; set; }
}
