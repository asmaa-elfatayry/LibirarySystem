using Domain.Entities;


namespace Application.Interfaces;

public interface IGenericService<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<bool> CreateAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
}
