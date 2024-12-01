using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres.AggregateRoots.Products;

public sealed class SellRepositoryPostgres(ProductDataContext context, ISellRepositoryCache sellRepositoryCache) : ISellRepositoryDatabase
{
    private readonly ProductDataContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ISellRepositoryCache _cache = sellRepositoryCache ?? throw new ArgumentNullException(nameof(sellRepositoryCache));

    public async Task<Results<FrozenSet<Sell>, BaseError>> Get(bool byPassCache = false, CancellationToken cancellation = default)
    {
        if (byPassCache)
            return await GetAllDatabase(cancellation);
        return await _cache.GetAllFromCache(cancellation);
    }

    public async Task<Results<FrozenSet<Sell>, BaseError>> Get(CancellationToken cancellation = default)
    {
        var result = await _cache.GetAllFromCache(cancellation);
        return await result.MatchAsync(async cache =>
        {
            if (cache is { Count: > 0 }) return cache;
            return await GetAllDatabase(cancellation);
        }, async e =>
        {
            return await GetAllDatabase(cancellation);
        });
    }

    public async Task<Results<Sell, BaseError>> GetById(Guid id, CancellationToken cancellation = default)
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
            return new BaseError($"Error while retrieving Sell with id '{id}': {ex.Message}", ex);
        }

        async Task<Results<Sell, BaseError>> GetFromDatabase()
        {
            var result = await _context.Sells.AsNoTracking().Include(x => x.Items).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellation);
            if (result is null)
                return (ResultStates.NotFound, new BaseError($"Sell '{id}' not found."));
            return result!;
        }
    }

    public async Task<ValidationResult> Insert(Sell sell, CancellationToken cancellation)
    {
        try
        {
            await _context.Sells.AddAsync(sell, cancellation);
            await _context.SaveChangesAsync(true);

            var cacheResult = await _cache.Insert(sell, cancellation);
            if (cacheResult != ValidationResult.Success)
            {
                // Log that the cache was not updated
            }

            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Sell '{sell.Id}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Update(Sell sell, CancellationToken cancellation)
    {
        try
        {
            _context.Sells.Update(sell);
            await _context.SaveChangesAsync(cancellation);
            
            var cacheResult = await _cache.Update(sell, cancellation);
            if (cacheResult != ValidationResult.Success)
            {
                // Log that the cache was not updated
            }
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating Sell '{sell.Id}': {ex.Message}");
        }
    }

    private async Task<Results<FrozenSet<Sell>, BaseError>> GetAllDatabase(CancellationToken cancellation = default)
    {
        try
        {
            var sells = await _context.Sells.AsNoTracking().Include(x => x.Items).AsNoTracking().ToListAsync(cancellation);
            if (sells is { Count: > 0 })
            {
                return sells.ToFrozenSet();
            }
            return FrozenSet<Sell>.Empty;
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving all Sells: {ex.Message}", ex);
        }
    }
}
