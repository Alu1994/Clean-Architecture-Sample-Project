using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Domain.Interfaces.Repositories;

public interface ICategoryRepositoryCache : ICategoryRepository
{
    Task<ValidationResult> InsertAll(FrozenSet<Category> categories, CancellationToken cancellation);
    Task<Validation<Error, FrozenSet<Category>>> GetAllFromCacheOrInsertFrom(Func<Task<Validation<Error, FrozenSet<Category>>>> func, CancellationToken cancellation);
}