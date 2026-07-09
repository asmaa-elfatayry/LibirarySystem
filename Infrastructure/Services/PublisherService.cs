using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class PublisherService : IPublisherService
{
    private readonly IGenericService<Publisher> _generic;

    public PublisherService(IGenericService<Publisher> generic)
    {
        _generic = generic;
    }

    public async Task<List<PublisherDto>> GetAllAsync()
    {
        var items = await _generic.GetAllAsync();
        return items.Select(i => new PublisherDto { Id = i.Id, Name = i.Name }).ToList();
    }

    public async Task<PublisherDto?> GetByIdAsync(Guid id)
    {
        var item = await _generic.GetByIdAsync(id);
        return item == null ? null : new PublisherDto { Id = item.Id, Name = item.Name };
    }

    public async Task<Result> CreateAsync(PublisherDto dto)
    {
        var entity = new Publisher { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id, Name = dto.Name };
        return await _generic.CreateAsync(entity);
    }

    public async Task<Result> UpdateAsync(PublisherDto dto)
    {
        var entity = new Publisher { Id = dto.Id, Name = dto.Name };
        return await _generic.UpdateAsync(entity);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        return await _generic.DeleteAsync(id);
    }
}
