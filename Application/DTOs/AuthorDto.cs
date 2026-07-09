using System;

namespace Application.DTOs;

public class AuthorDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Nationality { get; set; }
}
