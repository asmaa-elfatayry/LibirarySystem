using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime MembershipDate { get; set; } = DateTime.UtcNow;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
