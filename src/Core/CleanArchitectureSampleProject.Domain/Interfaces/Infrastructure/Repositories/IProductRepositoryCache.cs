using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

public interface IProductRepositoryCache : IProductRepository
{
    Task<ValidationResult> InsertAll(FrozenSet<Product> products, CancellationToken cancellation);
    Task<Validation<Error, FrozenSet<Product>>> GetAllFromCacheOrInsertFrom(Func<Task<Validation<Error, FrozenSet<Product>>>> func, CancellationToken cancellation);
}
