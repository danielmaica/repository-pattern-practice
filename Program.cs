using Microsoft.Extensions.Diagnostics.HealthChecks;
using RepositoryStore.Data;
using RepositoryStore.Models;
using RepositoryStore.Repositories;
using RepositoryStore.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoContext>();

// Health Check
builder.Services
  .AddHealthChecks()
  .AddMongoDb(
    sp => sp.GetRequiredService<MongoContext>().Database,
    "mongodb",
    HealthStatus.Unhealthy,
    new[] { "db", "nosql", "mongo" },
    TimeSpan.FromSeconds(5)
  );

// Injeção de Dependência dos Repositórios
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
  });
}

// Rotas Minimal API
app.MapHealthChecks("/health");

app.MapPost("/v1/products", async (CancellationToken ct, IProductRepository repo, string title) =>
{
  if (string.IsNullOrEmpty(title))
    return Results.BadRequest("Product title is empty");
  var product = new Product
  {
    Title = title
  };
  var result = await repo.CreateAsync(ct, product);
  return Results.Ok($"Product {result} created");
});

app.MapGet("/v1/products/{id}", async (CancellationToken ct, IProductRepository repo, string id) =>
{
  var product = await repo.GetByIdAsync(ct, id)
                ?? throw new Exception("Product not found.");
  return Results.Ok(product);
});

app.MapGet("/v1/products",
  async (CancellationToken ct, IProductRepository repo, string title = "", int skip = 0, int take = 10) =>
  {
    var products = await repo.PaginatedGetAllAsync(
                     ct,
                     p => p.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase),
                     skip,
                     take)
                   ?? throw new Exception("No products not found.");
    return Results.Ok(products);
  });

app.MapPut("/v1/products", async (CancellationToken ct, IProductRepository repo, Product product) =>
{
  var result = await repo.UpdateAsync(ct, product)
               ?? throw new Exception("Product not found.");
  return Results.Ok(result);
});

app.MapDelete("/v1/products/{id}", async (CancellationToken ct, IProductRepository repo, string id) =>
{
  var product = await repo.DeleteAsync(ct, id)
                ?? throw new Exception("Product not found.");
  return Results.Ok(product);
});

app.Run();