using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.ObjectModel;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Cache;

public sealed class SellItemRepositoryCache(IDistributedCache cache) : ISellItemRepositoryCache
{
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    private const string cacheKey = "SellItems";
    private readonly DistributedCacheEntryOptions _cacheTimeout = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiry
    };

    public async Task<Results<FrozenSet<SellItem>, BaseError>> Get(CancellationToken cancellation = default)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return FrozenSet<SellItem>.Empty;
            return JsonConvert.DeserializeObject<List<SellItem>>(cachedData)!.ToFrozenSet();
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving all Sells: {ex.Message}", ex);
        }
    }

    public async Task<Results<SellItem, BaseError>> GetById(Guid id, CancellationToken cancellation = default)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return new BaseError($"Error while retrieving SellItem with id '{id}'.");

            var results = JsonConvert.DeserializeObject<List<SellItem>>(cachedData);
            return results.First(x => x.Id == id);
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving SellItem with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<ValidationResult> Insert(SellItem sellItem, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
            {
                await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(new[] { sellItem }), _cacheTimeout, cancellation);
                return ValidationResult.Success!;
            }

            var previousCache = JsonConvert.DeserializeObject<List<SellItem>>(cachedData)!;
            previousCache.Add(sellItem);
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(previousCache), _cacheTimeout, cancellation);
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
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return new ValidationResult($"Error while Updating SellItem Id: '{sellItem.Id}', there are no sells on Database.");

            var previousCache = JsonConvert.DeserializeObject<List<SellItem>>(cachedData)!;
            if (previousCache is not { Count: >= 0 })
                return new ValidationResult($"Error while Updating SellItem, Id: '{sellItem.Id}' was not found on Database.");

            previousCache.RemoveAll(cat => cat.Id == sellItem.Id);
            previousCache.Add(sellItem);
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(previousCache), _cacheTimeout, cancellation);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating SellItem '{sellItem.Id}': {ex.Message}");
        }
    }

    // ====== I<T>RepositoryCache Methods ======
    public async Task<ValidationResult> InsertAll(FrozenSet<SellItem> sells, CancellationToken cancellation)
    {
        try
        {
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(sells), _cacheTimeout, cancellation);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting All Sells: {ex.Message}");
        }
    }

    public async Task<Results<FrozenSet<SellItem>, BaseError>> GetAllFromCache(CancellationToken cancellation)
    {
        var innerCache = await Get(cancellation: cancellation);
        return innerCache.Match<Results<FrozenSet<SellItem>, BaseError>>(cacheResult => cacheResult, e => e);
    }
    // ====== I<T>RepositoryCache Methods ======
}
