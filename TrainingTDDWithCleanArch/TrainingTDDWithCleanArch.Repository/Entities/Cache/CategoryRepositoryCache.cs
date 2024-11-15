using Microsoft.Extensions.Caching.Distributed;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products.Entities;
using TrainingTDDWithCleanArch.Domain.Interfaces;

namespace TrainingTDDWithCleanArch.Repository.Entities.Cache;

public sealed class CategoryRepositoryCache(IDistributedCache cache) : ICategoryRepository
{
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    private const string cacheKey = "Categories";

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
            return Error.New($"Error while retrieving all Products: {ex.Message}");
        }
    }

    public async Task<Validation<Error, Category>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return default;

            var categories = JsonConvert.DeserializeObject<List<Category>>(cachedData);
            return categories.First(x => x.Id == id);
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Category with id '{id}': {ex.Message}");
        }
    }

    public async Task<Validation<Error, Category>> GetByName(string categoryName, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return default;

            var categories = JsonConvert.DeserializeObject<List<Category>>(cachedData);
            return categories.First(x => x.Name == categoryName);
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Category with name '{categoryName}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Insert(Category category, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
            {
                await cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(new[] { category }), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiry
                });
                return ValidationResult.Success!;
            }

            var previousCache = JsonConvert.DeserializeObject<List<Category>>(cachedData)!;
            previousCache.Add(category);
            await cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(previousCache), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiry
            });
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Category '{category.Name}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Update(Category category, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
