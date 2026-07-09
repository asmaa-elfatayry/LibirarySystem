using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IGenericService<Category> _generic;

    public CategoryService(IGenericService<Category> generic)
    {
        _generic = generic;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var items = await _generic.GetAllAsync();
        return items.Select(i => new CategoryDto { Id = i.Id, Name = i.Name, Description = i.Description }).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var item = await _generic.GetByIdAsync(id);
        return item == null ? null : new CategoryDto { Id = item.Id, Name = item.Name, Description = item.Description };
    }

    public async Task<Result> CreateAsync(CategoryDto dto)
    {
        var entity = new Category { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id, Name = dto.Name, Description = dto.Description };
        return await _generic.CreateAsync(entity);
    }

    public async Task<Result> UpdateAsync(CategoryDto dto)
    {
        var entity = new Category { Id = dto.Id, Name = dto.Name, Description = dto.Description };
        return await _generic.UpdateAsync(entity);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        return await _generic.DeleteAsync(id);
    }
}
