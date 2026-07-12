
namespace Application.DTOs;

public class FineDto
{
    public Guid Id { get; set; }
    public Guid LoanId { get; set; }
    public string? BookTitle { get; set; }
    public string? MemberName { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime? PaidDate { get; set; }
}
