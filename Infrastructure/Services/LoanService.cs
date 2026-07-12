using Application.Common;
using Application.DTOs;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services;

public class LoanService : ILoanService
{
    private readonly IUnitOfWork _unitOfWork;
    private const int LoanPeriodDays = 14;

    public LoanService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    private const decimal FineRatePerDay = 5m;
    public async Task<Result> BorrowAsync(Guid memberId, Guid bookId)
    {
        // 1) دور على أي نسخة متاحة من نفس الكتاب
        var availableCopy = await _unitOfWork.BookCopies
            .GetAllQueryable(c => c.Book)
            .FirstOrDefaultAsync(c => c.BookId == bookId && c.Status == eCopyStatus.Available);

        if (availableCopy == null)
            return Result.Failure(eResultStatus.Conflict, "لا توجد نسخة متاحة لهذا الكتاب حاليًا");

        // 2) (اختياري لاحقًا) تأكد إن العضو مالوش استعارات متأخرة/غرامات معلقة
        var hasUnpaidFines = await _unitOfWork.Fines
    .GetAllQueryable(f => f.Loan)
    .AnyAsync(f => f.Loan.MemberId == memberId && !f.IsPaid);

        if (hasUnpaidFines)
            return Result.Failure(eResultStatus.Conflict,"لديك غرامات غير مسددة، يرجى السداد أولاً");

        // 3) اعمل Loan جديد
        var loan = new Loan
        {
            MemberId = memberId,
            BookCopyId = availableCopy.Id,
            BorrowDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(LoanPeriodDays),
            Status = LoanStatus.Active
        };

        await _unitOfWork.Loans.AddAsync(loan);

        // 4) غيّر حالة النسخة
        availableCopy.Status = eCopyStatus.Borrowed;
        _unitOfWork.BookCopies.Update(availableCopy);

        // 5) احفظ كل حاجة مع بعض - دي اللحظة اللي بيبان فيها فرق الـ UnitOfWork
        await _unitOfWork.SaveChangesAsync();

        return Result.Success("تم تسجيل الاستعارة بنجاح");
    }

    public async Task<Result> ReturnAsync(Guid loanId)
    {
        var loan = await _unitOfWork.Loans
            .GetAllQueryable(l => l.BookCopy)
            .FirstOrDefaultAsync(l => l.Id == loanId);

        if (loan == null)
            return Result.Failure(eResultStatus.NotFound,"عملية الاستعارة غير موجودة");

        if (loan.Status == LoanStatus.Returned)
            return Result.Failure(eResultStatus.Conflict,"الكتاب مُرجَع بالفعل");

        var returnDate = DateTime.UtcNow;
        var isLate = returnDate.Date > loan.DueDate.Date;

        loan.ReturnDate = returnDate;
        loan.Status = LoanStatus.Returned;
        _unitOfWork.Loans.Update(loan);

        string message = "تم تسجيل إرجاع الكتاب بنجاح";

        // ===== إنشاء غرامة تلقائي لو الإرجاع متأخر =====
        if (isLate)
        {
            var daysLate = (returnDate.Date - loan.DueDate.Date).Days;
            var fine = new Fine
            {
                LoanId = loan.Id,
                Amount = daysLate * FineRatePerDay,
                IsPaid = false,
                IssuedDate = returnDate
            };
            await _unitOfWork.Fines.AddAsync(fine);
            message = $"تم الإرجاع متأخرًا {daysLate} يوم - غرامة {fine.Amount} جنيه";
        }

        // ===== باقي منطق الحجز اللي عملناه قبل كده (زي ما هو) =====
        var bookCopy = loan.BookCopy;
        var bookId = bookCopy.BookId;

        // دور على أقدم حجز معلّق لنفس الكتاب (أول واحد في الطابور = FIFO)
        var nextReservation = await _unitOfWork.Reservations
            .GetAllQueryable()
            .Where(r => r.BookId == bookId && r.Status == eReservationStatus.Pending)
            .OrderBy(r => r.RequestDate)
            .FirstOrDefaultAsync();

        if (nextReservation != null)
        {
            var newLoan = new Loan
            {
                MemberId = nextReservation.MemberId,
                BookCopyId = bookCopy.Id,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(LoanPeriodDays),
                Status = LoanStatus.Active
            };
            await _unitOfWork.Loans.AddAsync(newLoan);

            nextReservation.Status = eReservationStatus.Fulfilled;
            _unitOfWork.Reservations.Update(nextReservation);

            bookCopy.Status = eCopyStatus.Borrowed;
            message += " - وتحويل النسخة تلقائيًا لصاحب الحجز التالي";
        }
        else
        {
            bookCopy.Status = eCopyStatus.Available;
        }

        _unitOfWork.BookCopies.Update(bookCopy);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success(message);
    }

    public async Task<List<LoanDto>> GetAllAsync()
    {
        var loans = await _unitOfWork.Loans
            .GetAllQueryable(l => l.Member, l => l.BookCopy, l => l.BookCopy.Book)
            .OrderByDescending(l => l.BorrowDate)
            .ToListAsync();

        return loans.Select(MapToDto).ToList();
    }

    public async Task<List<LoanDto>> GetActiveLoansForMemberAsync(Guid memberId)
    {
        var loans = await _unitOfWork.Loans
            .GetAllQueryable(l => l.BookCopy, l => l.BookCopy.Book)
            .Where(l => l.MemberId == memberId && l.Status == LoanStatus.Active)
            .ToListAsync();

        return loans.Select(MapToDto).ToList();
    }

    private static LoanDto MapToDto(Loan l) => new()
    {
        Id = l.Id,
        MemberId = l.MemberId,
        MemberName = l.Member?.FullName,
        BookCopyId = l.BookCopyId,
        BookTitle = l.BookCopy?.Book?.Title,
        CopyNumber = l.BookCopy?.CopyNumber,
        BorrowDate = l.BorrowDate,
        DueDate = l.DueDate,
        ReturnDate = l.ReturnDate,
        Status = l.Status.ToString()
    };
}
