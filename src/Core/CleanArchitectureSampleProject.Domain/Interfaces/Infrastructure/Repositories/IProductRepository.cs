using System.Collections.Frozen;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<Results<FrozenSet<Product>, BaseError>> Get(CancellationToken cancellation = default);
    Task<Results<Product, BaseError>> GetById(Guid id, CancellationToken cancellation);
    Task<Results<Product, BaseError>> GetByIdOrDefault(Guid id, CancellationToken cancellation);
    Task<Results<Product, BaseError>> GetByName(string productName, CancellationToken cancellation);
    Task<ValidationResult> Insert(Product product, CancellationToken cancellation);
    Task<ValidationResult> Update(Product product, CancellationToken cancellation);
}

public interface IProductRepositoryDatabase : IProductRepository
{
    Task<Results<FrozenSet<Product>, BaseError>> Get(bool byPassCache = false, CancellationToken cancellation = default);
}

public interface IProductRepositoryCache : IProductRepository
{
    Task<ValidationResult> InsertAll(FrozenSet<Product> products, CancellationToken cancellation);
    Task<Results<FrozenSet<Product>, BaseError>> GetAllFromCache(CancellationToken cancellation);
}