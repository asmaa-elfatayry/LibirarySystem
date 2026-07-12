
namespace Domain.Interfaces;


    public interface IUnitOfWork
    {
        IGenericRepository<Loan> Loans { get; }
        IGenericRepository<BookCopy> BookCopies { get; }
        IGenericRepository<Fine> Fines { get; }

    IGenericRepository<Reservation> Reservations { get; }
    Task<int> SaveChangesAsync();
    }

