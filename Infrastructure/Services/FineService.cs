using Application.Common;
using Application.DTOs;
using Domain.Enums;


namespace Infrastructure.Services;

public class FineService : IFineService
{
    private readonly IUnitOfWork _unitOfWork;

    public FineService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<FineDto>> GetAllAsync()
    {
        var fines = await _unitOfWork.Fines
            .GetAllQueryable(f => f.Loan, f => f.Loan.Member, f => f.Loan.BookCopy, f => f.Loan.BookCopy.Book)
            .OrderByDescending(f => f.IssuedDate)
            .ToListAsync();

        return fines.Select(MapToDto).ToList();
    }

    public async Task<List<FineDto>> GetMyFinesAsync(Guid memberId)
    {
        var fines = await _unitOfWork.Fines
            .GetAllQueryable(f => f.Loan, f => f.Loan.BookCopy, f => f.Loan.BookCopy.Book)
            .Where(f => f.Loan.MemberId == memberId)
            .OrderByDescending(f => f.IssuedDate)
            .ToListAsync();

        return fines.Select(MapToDto).ToList();
    }

    public async Task<bool> HasUnpaidFinesAsync(Guid memberId)
    {
        return await _unitOfWork.Fines
            .GetAllQueryable(f => f.Loan)
            .AnyAsync(f => f.Loan.MemberId == memberId && !f.IsPaid);
    }

    public async Task<Result> MarkAsPaidAsync(Guid fineId)
    {
        var fine = await _unitOfWork.Fines.GetByIdAsync(fineId);
        if (fine == null)
            return Result.Failure(eResultStatus.NotFound,"الغرامة غير موجودة" );

        if (fine.IsPaid)
            return Result.Failure(eResultStatus.Conflict,"الغرامة مدفوعة بالفعل");

        fine.IsPaid = true;
        fine.PaidDate = DateTime.UtcNow;
        _unitOfWork.Fines.Update(fine);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success("تم تسجيل السداد");
    }

    private static FineDto MapToDto(Fine f) => new()
    {
        Id = f.Id,
        LoanId = f.LoanId,
        BookTitle = f.Loan?.BookCopy?.Book?.Title,
        MemberName = f.Loan?.Member?.FullName,
        Amount = f.Amount,
        IsPaid = f.IsPaid,
        IssuedDate = f.IssuedDate,
        PaidDate = f.PaidDate
    };
}