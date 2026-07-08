using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public IQueryable<T> GetAllQueryable()
    => _dbSet.Where(e => !e.IsDeleted).AsQueryable();

    public async Task<bool> ExistsAsync(Guid id)
        => await _dbSet.AnyAsync(e => e.Id == id && !e.IsDeleted);

    public async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

    public async Task<List<T>> GetAllAsync()
        => await _dbSet.Where(e => !e.IsDeleted).ToListAsync();

    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Delete(T entity)
    {
        entity.IsDeleted = true;   // Soft Delete 
        _dbSet.Update(entity);
    }
}
