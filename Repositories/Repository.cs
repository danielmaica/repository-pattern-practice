using System.Linq.Expressions;
using MongoDB.Driver;
using RepositoryStore.Data;
using RepositoryStore.Models;
using RepositoryStore.Repositories.Interfaces;

namespace RepositoryStore.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
  private readonly IMongoCollection<T> _collection;

  public Repository(MongoContext context)
  {
    _collection = context.Database.GetCollection<T>(typeof(T).Name);
  }

  public virtual async Task<T> CreateAsync(CancellationToken ct, T entity)
  {
    await _collection.InsertOneAsync(entity, new InsertOneOptions(), ct);
    return entity;
  }

  public virtual async Task<T> GetAsync(CancellationToken ct, Expression<Func<T, bool>> predicate)
  {
    return await _collection.Find(predicate).FirstOrDefaultAsync(ct);
  }

  public virtual async Task<T> GetByIdAsync(CancellationToken ct, string id)
  {
    return await _collection.Find(Builders<T>.Filter.Eq(e => e.Id, id)).FirstOrDefaultAsync(ct);
  }

  public virtual async Task<List<T>> GetAllAsync()
  {
    return await _collection.Find(_ => true).ToListAsync();
  }

  public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
  {
    return await _collection.Find(predicate).ToListAsync();
  }

  public virtual async Task<List<T>> PaginatedGetAllAsync(CancellationToken ct, Expression<Func<T, bool>> predicate,
    int skip = 1,
    int take = 10)
  {
    return await _collection.Find(predicate).Skip(skip).Limit(take).ToListAsync(ct);
  }

  public virtual async Task<T> UpdateAsync(CancellationToken ct, T entity)
  {
    var options = new FindOneAndReplaceOptions<T>
    {
      ReturnDocument = ReturnDocument.After
    };
    entity = await _collection.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity, options, ct);
    return entity ?? throw new Exception("Entity not found");
  }

  public virtual async Task<string> DeleteAsync(CancellationToken ct, string id)
  {
    var entity = await _collection.FindOneAndDeleteAsync(e => e.Id == id, new FindOneAndDeleteOptions<T>(), ct);
    return "Register has been successfully deleted";
  }
}