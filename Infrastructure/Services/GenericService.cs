using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
namespace Infrastructure.Services;

    public class GenericService<T> : IGenericService<T> where T : BaseEntity
{
    private readonly IGenericRepository<T> _repository;
    private readonly AppDbContext _context;

    public GenericService(IGenericRepository<T> repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    => await _repository.GetAllQueryable(includes).ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        => await _repository.GetByIdAsync(id, includes);


    public async Task<List<T>> GetAllAsync()
        => await _repository.GetAllQueryable().ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id)
        => await _repository.GetByIdAsync(id);

    public async Task<Result> CreateAsync(T entity)
    {
        await _repository.AddAsync(entity);

        var saved = await _context.SaveChangesAsync() > 0;

        if (!saved)
            return Result.Failure(eResultStatus.Error, "Failed to create.");

        return Result.Success("Created successfully.");
    }

    public async Task<Result> UpdateAsync(T entity)
    {
        var exists = await _repository.ExistsAsync(entity.Id);

        if (!exists)
            return Result.Failure(eResultStatus.NotFound, "Item not found.");

        _repository.Update(entity);

        var saved = await _context.SaveChangesAsync() > 0;

        if (!saved)
            return Result.Failure(eResultStatus.Error, "Failed to update.");

        return Result.Success("Updated successfully.");
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity == null)
            return Result.Failure(eResultStatus.NotFound, "Item not found.");

        _repository.Delete(entity);

        var saved = await _context.SaveChangesAsync() > 0;

        if (!saved)
            return Result.Failure(eResultStatus.Error, "Failed to delete.");

        return Result.Success("Deleted successfully.");
    }
}

