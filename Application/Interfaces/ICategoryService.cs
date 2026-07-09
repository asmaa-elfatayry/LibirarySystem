using Application.Common;
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<Result> CreateAsync(CategoryDto dto);
    Task<Result> UpdateAsync(CategoryDto dto);
    Task<Result> DeleteAsync(Guid id);
}
