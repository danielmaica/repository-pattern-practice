using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace RepositoryStore.Data;

public class MongoContext
{
  public MongoContext(IOptions<MongoDbSettings> settings)
  {
    var client = new MongoClient(settings.Value.ConnectionString);
    Database = client.GetDatabase(settings.Value.DatabaseName);
  }

  public IMongoDatabase Database { get; }
}