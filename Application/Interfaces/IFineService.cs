using Application.Common;
using Application.DTOs;


namespace Application.Interfaces;

public interface IFineService
{
    Task<List<FineDto>> GetAllAsync();
    Task<List<FineDto>> GetMyFinesAsync(Guid memberId);
    Task<bool> HasUnpaidFinesAsync(Guid memberId);
    Task<Result> MarkAsPaidAsync(Guid fineId);
}
