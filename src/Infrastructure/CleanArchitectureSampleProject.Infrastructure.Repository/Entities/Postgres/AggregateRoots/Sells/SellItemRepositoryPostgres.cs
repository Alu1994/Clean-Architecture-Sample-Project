using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres.AggregateRoots.Products;

public sealed class SellItemRepositoryPostgres(ProductDataContext context, ISellItemRepositoryCache sellItemRepositoryCache) : ISellItemRepositoryDatabase
{
    private readonly ProductDataContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ISellItemRepositoryCache _cache = sellItemRepositoryCache ?? throw new ArgumentNullException(nameof(sellItemRepositoryCache));

    public async Task<Results<FrozenSet<SellItem>, BaseError>> Get(bool byPassCache = false, CancellationToken cancellation = default)
    {
        if (byPassCache)
            return await GetAllDatabase();
        return await _cache.GetAllFromCache(cancellation);
    }

    public async Task<Results<FrozenSet<SellItem>, BaseError>> Get(CancellationToken cancellation = default)
    {
        var result = await _cache.GetAllFromCache(cancellation);
        return await result.MatchAsync(async cache =>
        {
            if (cache is not null) return cache;
            return await GetAllDatabase(cancellation);
        }, async e =>
        {
            return await GetAllDatabase(cancellation);
        });
    }

    public async Task<Results<SellItem, BaseError>> GetById(Guid id, CancellationToken cancellation = default)
    {
        try
        {
            var cacheResult = await _cache.GetById(id, cancellation);
            return await cacheResult.MatchAsync(async cache =>
            {
                if (cache is not null) return cache;
                return await GetFromDatabase();
            }, async e =>
            {
                return await GetFromDatabase();
            });
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving SellItem with id '{id}': {ex.Message}", ex);
        }

        async Task<Results<SellItem, BaseError>> GetFromDatabase()
        {
            var result = await _context.SellItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellation);
            if (result is null)
                return (ResultStates.NotFound, new BaseError($"SellItem '{id}' not found."));
            return result!;
        }
    }

    public async Task<ValidationResult> Insert(SellItem sellItem, CancellationToken cancellation)
    {
        try
        {
            var cacheResult = await _cache.Insert(sellItem, cancellation);
            if (cacheResult != ValidationResult.Success)
            {
                // Log that the cache was not updated
            }

            await _context.SellItems.AddAsync(sellItem, cancellation);
            await _context.SaveChangesAsync(true);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting SellItem '{sellItem.Id}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Update(SellItem sellItem, CancellationToken cancellation)
    {
        try
        {
            var cacheResult = await _cache.Update(sellItem, cancellation);
            if (cacheResult != ValidationResult.Success)
            {
                // Log that the cache was not updated
            }

            _context.SellItems.Update(sellItem);
            await _context.SaveChangesAsync(cancellation);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating SellItem '{sellItem.Id}': {ex.Message}");
        }
    }

    private async Task<Results<FrozenSet<SellItem>, BaseError>> GetAllDatabase(CancellationToken cancellation = default)
    {
        try
        {
            var results = await _context.SellItems.AsNoTracking().ToListAsync(cancellation);
            if (results is null)
            {
                return FrozenSet<SellItem>.Empty;
            }
            return results.ToFrozenSet();
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving all SellItems: {ex.Message}", ex);
        }
    }
}
