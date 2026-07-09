using Application.Common;
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAllAsync();
    Task<AuthorDto?> GetByIdAsync(Guid id);
    Task<Result> CreateAsync(AuthorDto dto);
    Task<Result> UpdateAsync(AuthorDto dto);
    Task<Result> DeleteAsync(Guid id);
}
