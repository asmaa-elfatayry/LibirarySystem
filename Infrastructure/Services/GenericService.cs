using Application.Interfaces;
using Domain.Entities;
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

    public async Task<bool> CreateAsync(T entity)
    {
        await _repository.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        var exists = await _repository.ExistsAsync(entity.Id);
        if (!exists) return false;

        _repository.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        _repository.Delete(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}

