using Application.Common;
using Domain.Entities;
using System.Linq.Expressions;


namespace Application.Interfaces;

public interface IGenericService<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<Result> CreateAsync(T entity);
    Task<Result> UpdateAsync(T entity);
    Task<Result> DeleteAsync(Guid id);
}
