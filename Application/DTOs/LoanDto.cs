using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs;

public class LoanDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public string? MemberName { get; set; }

    public Guid BookCopyId { get; set; }
    public string? BookTitle { get; set; }
    public string? CopyNumber { get; set; }

    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
