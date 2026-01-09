using RepositoryStore.Data;
using RepositoryStore.Models;
using RepositoryStore.Repositories.Interfaces;

namespace RepositoryStore.Repositories;

public class ProductRepository(MongoContext context) : Repository<Product>(context), IProductRepository
{
}