using Microsoft.Extensions.Caching.Distributed;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Cache;

public sealed class CategoryRepositoryCache(IDistributedCache cache) : ICategoryRepositoryCache
{
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    private const string cacheKey = "Categories";
    private readonly DistributedCacheEntryOptions _cacheTimeout = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiry
    };

    public async Task<Results<FrozenSet<Category>, BaseError>> Get(CancellationToken cancellation = default)
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
            return new BaseError($"Error while retrieving all Categories: {ex.Message}", ex);
        }
    }

    public async Task<Results<Category, BaseError>> GetById(Guid id, CancellationToken cancellation = default)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return new BaseError($"Error while retrieving Category with id '{id}'.");

            var categories = JsonConvert.DeserializeObject<List<Category>>(cachedData);
            return categories.First(x => x.Id == id);
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Category with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Results<Category, BaseError>> GetByIdOrDefault(Guid id, CancellationToken cancellation = default)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return new BaseError($"Error while retrieving Category with id '{id}'.");

            var categories = JsonConvert.DeserializeObject<List<Category>>(cachedData);
            return categories.FirstOrDefault(x => x.Id == id);
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Category with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Results<Category, BaseError>> GetByName(string categoryName, CancellationToken cancellation = default)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return new BaseError($"Error while retrieving Category with name '{categoryName}'.");

            var categories = JsonConvert.DeserializeObject<List<Category>>(cachedData)!;
            var result = categories.FirstOrDefault(x => x.Name == categoryName);

            if (result is null)
                return (ResultStates.NotFound, new BaseError($"Category '{categoryName}' not found."));
            return result;
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Category with name '{categoryName}': {ex.Message}", ex);
        }
    }

    public async Task<ValidationResult> Insert(Category category, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
            {
                await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(new[] { category }), _cacheTimeout, cancellation);
                return ValidationResult.Success!;
            }

            var previousCache = JsonConvert.DeserializeObject<List<Category>>(cachedData)!;
            previousCache.Add(category);
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(previousCache), _cacheTimeout, cancellation);
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
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(previousCache), _cacheTimeout, cancellation);
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
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(categories), _cacheTimeout, cancellation);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting All Categories: {ex.Message}");
        }
    }

    public async Task<Results<FrozenSet<Category>, BaseError>> GetAllFromCache(CancellationToken cancellation)
    {
        var cacheCategories = await Get(cancellation: cancellation);
        return cacheCategories.Match<Results<FrozenSet<Category>, BaseError>>(cache => cache, e => e);
    }
    // ====== ICategoryRepositoryCache Methods ======
}
