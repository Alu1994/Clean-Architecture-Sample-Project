using Microsoft.Extensions.Caching.Distributed;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Cache;

public sealed class CategoryRepositoryCache(IDistributedCache cache) : ICategoryRepository, ICategoryRepositoryCache
{
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    private const string cacheKey = "Categories";
    private readonly DistributedCacheEntryOptions _cacheTimeout = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiry
    };

    public async Task<Validation<Error, FrozenSet<Category>>> Get(CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return Enumerable.Empty<Category>().ToFrozenSet();
            return JsonConvert.DeserializeObject<List<Category>>(cachedData)!.ToFrozenSet();
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving all Categories: {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Category>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return Error.New($"Error while retrieving Category with id '{id}'.");

            var categories = JsonConvert.DeserializeObject<List<Category>>(cachedData);
            return categories.First(x => x.Id == id);
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Category with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Category>> GetByName(string categoryName, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return Error.New($"Error while retrieving Category with name '{categoryName}'.");

            var categories = JsonConvert.DeserializeObject<List<Category>>(cachedData);
            return categories.First(x => x.Name == categoryName);
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Category with name '{categoryName}': {ex.Message}", ex);
        }
    }

    public async Task<ValidationResult> Insert(Category category, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
            {
                await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(new[] { category }), _cacheTimeout);
                return ValidationResult.Success!;
            }

            var previousCache = JsonConvert.DeserializeObject<List<Category>>(cachedData)!;
            previousCache.Add(category);
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(previousCache), _cacheTimeout);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Category '{category.Name}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Update(Category category, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return new ValidationResult($"Error while Updating Category Id: '{category.Id}', there are no categories on Database.");

            var previousCache = JsonConvert.DeserializeObject<List<Category>>(cachedData)!;
            if (previousCache is not { Count: >= 0 })
                return new ValidationResult($"Error while Updating Category, Id: '{category.Id}' was not found on Database.");

            previousCache.RemoveAll(cat => cat.Id == category.Id);
            previousCache.Add(category);
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(previousCache), _cacheTimeout);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating Category '{category.Name}': {ex.Message}");
        }
    }

    // ====== ICategoryRepositoryCache Methods ======
    public async Task<ValidationResult> InsertAll(FrozenSet<Category> categories, CancellationToken cancellation)
    {
        try
        {
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(categories), _cacheTimeout);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting All Categories: {ex.Message}");
        }
    }

    public async Task<Validation<Error, FrozenSet<Category>>> GetAllFromCacheOrInsertFrom(Func<Task<Validation<Error, FrozenSet<Category>>>> func, CancellationToken cancellation)
    {
        var cacheCategories = await Get(cancellation);
        return await cacheCategories.MatchAsync(async cache =>
        {
            if (cache is not { Count: > 0 })
            {
                var categories = await func();
                return await categories.MatchAsync<Validation<Error, FrozenSet<Category>>>(async s =>
                {
                    await InsertAll(s.ToFrozenSet(), cancellation);
                    return s.ToFrozenSet();
                }, er => er);
            }
            return cache;
        }, e => e);
    }
    // ====== ICategoryRepositoryCache Methods ======
}
