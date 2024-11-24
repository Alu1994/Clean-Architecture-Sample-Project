using System.Collections.Frozen;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;

namespace CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

public interface ICategoryRepository
{
    Task<Validation<Error, FrozenSet<Category>>> Get(CancellationToken cancellation = default);
    Task<Validation<Error, Category>> GetById(Guid id, CancellationToken cancellation = default);
    Task<Results<Category, Error>> GetByName(string categoryName, CancellationToken cancellation = default);
    Task<ValidationResult> Insert(Category category, CancellationToken cancellation);
    Task<ValidationResult> Update(Category category, CancellationToken cancellation);
}

public interface ICategoryRepositoryDatabase : ICategoryRepository
{
    Task<Validation<Error, FrozenSet<Category>>> Get(bool byPassCache = false, CancellationToken cancellation = default);
}

public interface ICategoryRepositoryCache : ICategoryRepository
{
    Task<ValidationResult> InsertAll(FrozenSet<Category> categories, CancellationToken cancellation);
    Task<Validation<Error, FrozenSet<Category>>> GetAllFromCache(CancellationToken cancellation);
}