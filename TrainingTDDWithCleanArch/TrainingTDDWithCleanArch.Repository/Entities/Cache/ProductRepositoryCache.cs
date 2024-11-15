using Microsoft.Extensions.Caching.Distributed;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products;
using TrainingTDDWithCleanArch.Domain.Interfaces;

namespace TrainingTDDWithCleanArch.Repository.Entities.Cache;

public sealed class ProductRepositoryCache(IDistributedCache cache) : IProductRepository
{
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    private const string cacheKey = "Products";

    public async Task<Validation<Error, FrozenSet<Product>>> Get(CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return Enumerable.Empty<Product>().ToFrozenSet();
            return JsonConvert.DeserializeObject<List<Product>>(cachedData)!.ToFrozenSet();
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving all Products: {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Product>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return default;

            var products = JsonConvert.DeserializeObject<List<Product>>(cachedData);
            return products.First(x => x.Id == id);
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Product with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Product>> GetByName(string productName, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return default;

            var products = JsonConvert.DeserializeObject<List<Product>>(cachedData);
            return products.First(x => x.Name == productName);
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Product with name '{productName}': {ex.Message}", ex);
        }
    }

    public async Task<ValidationResult> Insert(Product product, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
            {
                await cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(new[] { product }), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiry
                });
                return ValidationResult.Success!;
            }

            var previousCache = JsonConvert.DeserializeObject<List<Product>>(cachedData)!;
            previousCache.Add(product);

            await cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(previousCache), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiry
            });
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Product '{product.Name}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Update(Product product, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
