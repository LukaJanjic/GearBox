using Core.Domain.Entities;
using Core.Specifications;

namespace Core.Interfaces.Repositories;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync(ISpecification<T> spec);
    Task<T?> GetEntityAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);
}
