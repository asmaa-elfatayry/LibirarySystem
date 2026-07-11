using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Domain.Entities;

public class Loan:BaseEntity
{
    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.Active;

    // Foreign Key: Member
    public Guid MemberId { get; set; }
    public virtual ApplicationUser Member { get; set; } = null!;

    // Foreign Key: BookCopy
    public Guid BookCopyId { get; set; }
    public virtual BookCopy BookCopy { get; set; } = null!;

    public virtual Fine? Fine { get; set; }
}
