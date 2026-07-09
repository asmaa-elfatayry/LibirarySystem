using Application.Common;
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IBookService
{
    Task<List<BookDto>> GetAllAsync();
    Task<BookDto?> GetByIdAsync(Guid id);
    Task<Result> CreateAsync(BookDto dto);
    Task<Result> UpdateAsync(BookDto dto);
    Task<Result> DeleteAsync(Guid id);
}
