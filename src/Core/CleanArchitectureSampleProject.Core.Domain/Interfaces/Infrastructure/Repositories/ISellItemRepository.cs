using System.Collections.Frozen;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using CleanArchitectureSampleProject.CrossCuttingConcerns;

namespace CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

public interface ISellItemRepository
{
    Task<Results<FrozenSet<SellItem>, BaseError>> Get(CancellationToken cancellation = default);
    Task<Results<SellItem, BaseError>> GetById(Guid id, CancellationToken cancellation);
    Task<ValidationResult> Insert(SellItem sellItem, CancellationToken cancellation);
    Task<ValidationResult> Update(SellItem sellItem, CancellationToken cancellation);
}

public interface ISellItemRepositoryDatabase : ISellItemRepository
{
    Task<Results<FrozenSet<SellItem>, BaseError>> Get(bool byPassCache = false, CancellationToken cancellation = default);
}

public interface ISellItemRepositoryCache : ISellItemRepository
{
    Task<ValidationResult> InsertAll(FrozenSet<SellItem> sells, CancellationToken cancellation);
    Task<Results<FrozenSet<SellItem>, BaseError>> GetAllFromCache(CancellationToken cancellation);
}