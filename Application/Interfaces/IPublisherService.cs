using Application.Common;
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IPublisherService
{
    Task<List<PublisherDto>> GetAllAsync();
    Task<PublisherDto?> GetByIdAsync(Guid id);
    Task<Result> CreateAsync(PublisherDto dto);
    Task<Result> UpdateAsync(PublisherDto dto);
    Task<Result> DeleteAsync(Guid id);
}
