using RepositoryStore.Data;
using RepositoryStore.Models;
using RepositoryStore.Repositories.Interfaces;

namespace RepositoryStore.Repositories;

public class ProductRepository(MongoDbContext context) : Repository<Product>(context), IProductRepository
{
}