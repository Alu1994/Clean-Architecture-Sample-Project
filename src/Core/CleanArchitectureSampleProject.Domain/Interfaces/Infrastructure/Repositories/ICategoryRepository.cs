using System.Collections.Frozen;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.CrossCuttingConcerns;

namespace CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

public interface ICategoryRepository
{
    Task<Results<FrozenSet<Category>, BaseError>> Get(CancellationToken cancellation = default);
    Task<Results<Category, BaseError>> GetById(Guid id, CancellationToken cancellation = default);
    Task<Results<Category, BaseError>> GetByName(string categoryName, CancellationToken cancellation = default);
    Task<ValidationResult> Insert(Category category, CancellationToken cancellation);
    Task<ValidationResult> Update(Category category, CancellationToken cancellation);
}

public interface ICategoryRepositoryDatabase : ICategoryRepository
{
    Task<Results<FrozenSet<Category>, BaseError>> Get(bool byPassCache = false, CancellationToken cancellation = default);
}

public interface ICategoryRepositoryCache : ICategoryRepository
{
    Task<ValidationResult> InsertAll(FrozenSet<Category> categories, CancellationToken cancellation);
    Task<Results<FrozenSet<Category>, BaseError>> GetAllFromCache(CancellationToken cancellation);
}