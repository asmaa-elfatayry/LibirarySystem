using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;

    private IGenericRepository<Loan>? _loans;
    private IGenericRepository<BookCopy>? _bookCopies;
    private IGenericRepository<Fine>? _fines;
    private IGenericRepository<Reservation>? _reservations;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Lazy Initialization: الـ Repository بيتعمل بس أول مرة تستخدمها فعليًا
    public IGenericRepository<Loan> Loans =>
        _loans ??= new GenericRepository<Loan>(_context);

    public IGenericRepository<BookCopy> BookCopies =>
        _bookCopies ??= new GenericRepository<BookCopy>(_context);

    public IGenericRepository<Fine> Fines =>
        _fines ??= new GenericRepository<Fine>(_context);

    public IGenericRepository<Reservation> Reservations =>
        _reservations ??= new GenericRepository<Reservation>(_context);

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
      => _context.Dispose();
}