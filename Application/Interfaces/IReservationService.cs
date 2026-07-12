using Application.Common;
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface IReservationService
{
    Task<Result> CreateAsync(Guid memberId, Guid bookId);
    Task<Result> CancelAsync(Guid reservationId, Guid requestingMemberId, bool isStaff);
    Task<List<ReservationDto>> GetMyReservationsAsync(Guid memberId);
    Task<List<ReservationDto>> GetAllAsync();
}
