

using Domain.Enums;

namespace Domain.Entities
{
    public class BookCopy:BaseEntity
    {
        public string CopyNumber { get; set; } = string.Empty;
        public eCopyStatus Status { get; set; } = eCopyStatus.Available;

        // Foreign Key
        public Guid BookId { get; set; }
        public virtual Book Book { get; set; } = null!;

        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
