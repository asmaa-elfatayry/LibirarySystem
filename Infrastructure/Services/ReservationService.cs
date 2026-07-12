using Application.Common;
using Application.DTOs;
using Domain.Enums;

namespace Infrastructure.Services;

public class ReservationService : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> CreateAsync(Guid memberId, Guid bookId)
    {
        // لو فيه نسخة متاحة أصلاً، الحجز مالوش لازمة - يستعير مباشرة
        var hasAvailableCopy = await _unitOfWork.BookCopies
            .GetAllQueryable()
            .AnyAsync(c => c.BookId == bookId && c.Status == eCopyStatus.Available);

        if (hasAvailableCopy)
            return Result.Failure(eResultStatus.Conflict,"يوجد نسخة متاحة حاليًا، يمكنك استعارتها مباشرة" );

        // منع نفس العضو من حجز نفس الكتاب مرتين
        var alreadyReserved = await _unitOfWork.Reservations
            .GetAllQueryable()
            .AnyAsync(r => r.MemberId == memberId && r.BookId == bookId && r.Status == eReservationStatus.Pending);

        if (alreadyReserved)
            return Result.Failure(eResultStatus.Conflict, "لديك حجز قائم بالفعل لهذا الكتاب");

        var reservation = new Reservation
        {
            MemberId = memberId,
            BookId = bookId,
            RequestDate = DateTime.UtcNow,
            Status = eReservationStatus.Pending
        };

        await _unitOfWork.Reservations.AddAsync(reservation);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success("تم تسجيل الحجز، هنبلغك لما تتوفر نسخة");
    }

    public async Task<Result> CancelAsync(Guid reservationId, Guid requestingMemberId, bool isStaff)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
        if (reservation == null)
            return Result.Failure(eResultStatus.NotFound,"الحجز غير موجود" );

        if (!isStaff && reservation.MemberId != requestingMemberId)
            return Result.Failure( eResultStatus.Forbidden, "غير مصرح لك بإلغاء هذا الحجز");

        if (reservation.Status != eReservationStatus.Pending)
            return Result.Failure(eResultStatus.Conflict,"لا يمكن إلغاء حجز غير معلّق");

        reservation.Status = eReservationStatus.Cancelled;
        _unitOfWork.Reservations.Update(reservation);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success("تم إلغاء الحجز");
    }

    public async Task<List<ReservationDto>> GetMyReservationsAsync(Guid memberId)
    {
        var reservations = await _unitOfWork.Reservations
            .GetAllQueryable(r => r.Book)
            .Where(r => r.MemberId == memberId)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();

        return reservations.Select(MapToDto).ToList();
    }

    public async Task<List<ReservationDto>> GetAllAsync()
    {
        var reservations = await _unitOfWork.Reservations
            .GetAllQueryable(r => r.Book, r => r.Member)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();

        return reservations.Select(MapToDto).ToList();
    }

    private static ReservationDto MapToDto(Reservation r) => new()
    {
        Id = r.Id,
        MemberId = r.MemberId,
        MemberName = r.Member?.FullName,
        BookId = r.BookId,
        BookTitle = r.Book?.Title,
        RequestDate = r.RequestDate,
        Status = r.Status.ToString()
    };
}