using Domain.Entities;


namespace Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    IQueryable<T> GetAllQueryable();
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
