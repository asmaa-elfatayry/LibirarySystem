
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext:IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Fine> Fines { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // هنا هنضيف Configurations إضافية لاحقًا (Fluent API)
        builder.Entity<Book>()
      .HasOne(b => b.Author)
      .WithMany(a => a.Books)
      .HasForeignKey(b => b.AuthorId)
      .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BookCopy>()
            .HasOne(bc => bc.Book)
            .WithMany(b => b.Copies)
            .HasForeignKey(bc => bc.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Loan>()
            .HasOne(l => l.BookCopy)
            .WithMany(bc => bc.Loans)
            .HasForeignKey(l => l.BookCopyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Loan>()
            .HasOne(l => l.Member)
            .WithMany()
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Fine>()
            .HasOne(f => f.Loan)
            .WithOne()
            .HasForeignKey<Fine>(f => f.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Reservation>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reservations)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reservation>()
            .HasOne(r => r.Member)
            .WithMany()
            .HasForeignKey(r => r.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }


}
