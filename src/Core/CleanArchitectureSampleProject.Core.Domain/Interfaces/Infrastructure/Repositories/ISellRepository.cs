using System.Collections.Frozen;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.CrossCuttingConcerns;

namespace CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

public interface ISellRepository
{
    Task<Results<FrozenSet<Sell>, BaseError>> Get(CancellationToken cancellation = default);
    Task<Results<Sell, BaseError>> GetById(Guid id, CancellationToken cancellation);
    Task<ValidationResult> Insert(Sell sell, CancellationToken cancellation);
    Task<ValidationResult> Update(Sell sell, CancellationToken cancellation);
}

public interface ISellRepositoryDatabase : ISellRepository
{
    Task<Results<FrozenSet<Sell>, BaseError>> Get(bool byPassCache = false, CancellationToken cancellation = default);
}

public interface ISellRepositoryCache : ISellRepository
{
    Task<ValidationResult> InsertAll(FrozenSet<Sell> sells, CancellationToken cancellation);
    Task<Results<FrozenSet<Sell>, BaseError>> GetAllFromCache(CancellationToken cancellation);
}