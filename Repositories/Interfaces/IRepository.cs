using System.Linq.Expressions;

namespace RepositoryStore.Repositories.Interfaces;

public interface IRepository<T>
{
  Task<T> CreateAsync(CancellationToken ct, T entity);
  Task<T> GetAsync(CancellationToken ct, Expression<Func<T, bool>> predicate);
  Task<T> GetByIdAsync(CancellationToken ct, string id);
  Task<List<T>> GetAllAsync();
  Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

  Task<List<T>> PaginatedGetAllAsync(CancellationToken ct, Expression<Func<T, bool>> predicate, int skip = 1,
    int take = 10);

  Task<T> UpdateAsync(CancellationToken ct, T entity);
  Task<string> DeleteAsync(CancellationToken ct, string id);
}