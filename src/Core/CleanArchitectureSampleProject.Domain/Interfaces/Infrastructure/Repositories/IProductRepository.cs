using System.Collections.Frozen;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<Validation<Error, FrozenSet<Product>>> Get(CancellationToken cancellation = default);
    Task<Validation<Error, Product>> GetById(Guid id, CancellationToken cancellation);
    Task<Validation<Error, Product?>> GetByIdOrDefault(Guid id, CancellationToken cancellation);
    Task<Results<Product, Error>> GetByName(string productName, CancellationToken cancellation);
    Task<ValidationResult> Insert(Product product, CancellationToken cancellation);
    Task<ValidationResult> Update(Product product, CancellationToken cancellation);
}

public interface IProductRepositoryDatabase : IProductRepository
{
    Task<Validation<Error, FrozenSet<Product>>> Get(bool byPassCache = false, CancellationToken cancellation = default);
}

public interface IProductRepositoryCache : IProductRepository
{
    Task<ValidationResult> InsertAll(FrozenSet<Product> products, CancellationToken cancellation);
    Task<Validation<Error, FrozenSet<Product>>> GetAllFromCache(CancellationToken cancellation);
}