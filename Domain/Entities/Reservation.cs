using Domain.Enums;


namespace Domain.Entities;

public class Reservation : BaseEntity
{
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public eReservationStatus Status { get; set; } = eReservationStatus.Pending;

    public Guid MemberId { get; set; }
    public ApplicationUser Member { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
}
