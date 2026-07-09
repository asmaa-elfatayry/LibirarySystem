using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class AuthorService : IAuthorService
{
    private readonly IGenericService<Author> _generic;

    public AuthorService(IGenericService<Author> generic)
    {
        _generic = generic;
    }

    public async Task<List<AuthorDto>> GetAllAsync()
    {
        var items = await _generic.GetAllAsync();
        return items.Select(i => new AuthorDto { Id = i.Id, FullName = i.FullName, Bio = i.Bio, Nationality = i.Nationality }).ToList();
    }

    public async Task<AuthorDto?> GetByIdAsync(Guid id)
    {
        var item = await _generic.GetByIdAsync(id);
        return item == null ? null : new AuthorDto { Id = item.Id, FullName = item.FullName, Bio = item.Bio, Nationality = item.Nationality };
    }

    public async Task<Result> CreateAsync(AuthorDto dto)
    {
        var entity = new Author { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id, FullName = dto.FullName, Bio = dto.Bio, Nationality = dto.Nationality };
        return await _generic.CreateAsync(entity);
    }

    public async Task<Result> UpdateAsync(AuthorDto dto)
    {
        var entity = new Author { Id = dto.Id, FullName = dto.FullName, Bio = dto.Bio, Nationality = dto.Nationality };
        return await _generic.UpdateAsync(entity);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        return await _generic.DeleteAsync(id);
    }
}
