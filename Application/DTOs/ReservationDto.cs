

namespace Application.DTOs;

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public string? MemberName { get; set; }
    public Guid BookId { get; set; }
    public string? BookTitle { get; set; }
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
