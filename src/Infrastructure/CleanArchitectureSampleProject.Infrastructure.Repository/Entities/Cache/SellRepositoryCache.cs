using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.ObjectModel;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Cache;

public sealed class SellRepositoryCache(IDistributedCache cache) : ISellRepositoryCache
{
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    private const string cacheKey = "Sells";
    private readonly DistributedCacheEntryOptions _cacheTimeout = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiry
    };

    public async Task<Results<FrozenSet<Sell>, BaseError>> Get(CancellationToken cancellation = default)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return FrozenSet<Sell>.Empty;
            return JsonConvert.DeserializeObject<List<Sell>>(cachedData)!.ToFrozenSet();
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving all Sells: {ex.Message}", ex);
        }
    }

    public async Task<Results<Sell, BaseError>> GetById(Guid id, CancellationToken cancellation = default)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return new BaseError($"Error while retrieving Sell with id '{id}'.");

            var results = JsonConvert.DeserializeObject<List<Sell>>(cachedData);
            return results.First(x => x.Id == id);
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Sell with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<ValidationResult> Insert(Sell sell, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
            {
                await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(new[] { sell }), _cacheTimeout, cancellation);
                return ValidationResult.Success!;
            }

            var previousCache = JsonConvert.DeserializeObject<List<Sell>>(cachedData)!;
            previousCache.Add(sell);
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(previousCache), _cacheTimeout, cancellation);
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
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return new ValidationResult($"Error while Updating Sell Id: '{sell.Id}', there are no sells on Database.");

            var previousCache = JsonConvert.DeserializeObject<List<Sell>>(cachedData)!;
            if (previousCache is not { Count: >= 0 })
                return new ValidationResult($"Error while Updating Sell, Id: '{sell.Id}' was not found on Database.");

            previousCache.RemoveAll(cat => cat.Id == sell.Id);
            previousCache.Add(sell);
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(previousCache), _cacheTimeout, cancellation);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating Sell '{sell.Id}': {ex.Message}");
        }
    }

    // ====== I<T>RepositoryCache Methods ======
    public async Task<ValidationResult> InsertAll(FrozenSet<Sell> sells, CancellationToken cancellation)
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

    public async Task<Results<FrozenSet<Sell>, BaseError>> GetAllFromCache(CancellationToken cancellation)
    {
        var innerCache = await Get(cancellation: cancellation);
        return innerCache.Match<Results<FrozenSet<Sell>, BaseError>>(cacheResult => cacheResult, e => e);
    }
    // ====== I<T>RepositoryCache Methods ======
}
