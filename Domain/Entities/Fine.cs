using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public class Fine : BaseEntity
{
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; } = false;
    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
    public DateTime? PaidDate { get; set; }

    // Foreign Key: Loan (One-to-One)
    public Guid LoanId { get; set; }
    public virtual Loan Loan { get; set; } = null!;
}
