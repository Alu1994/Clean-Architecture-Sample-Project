using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.Extensions.Caching.Distributed;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Repository.Entities.Cache;

public sealed class ProductRepositoryCache(ILogger<ProductRepositoryCache> logger, ICategoryRepository categoryRepository, IDistributedCache cache) : IProductRepository, IProductRepositoryCache
{
    private readonly ILogger<ProductRepositoryCache> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    private const string cacheKey = "Products";
    private readonly DistributedCacheEntryOptions _cacheTimeout = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiry
    };

    public async Task<Validation<Error, FrozenSet<Product>>> Get(CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return Enumerable.Empty<Product>().ToFrozenSet();

            var products = JsonConvert.DeserializeObject<List<Product>>(cachedData, JsonSerializationOptions.RemoveInfiniteLoop)!.ToFrozenSet();
            foreach (var product in products)
            {
                var categoryResult = await _categoryRepository.GetById(product.CategoryId, cancellation);
                _ = categoryResult.Match<Validation<Error, Product>>(category =>
                {
                    return product.WithCategory(category);
                }, erCat =>
                {
                    _logger.LogSeqError(erCat);
                    return erCat;
                });
            }
            return products;
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
            var products = await Get(cancellation);
            return products.Match<Validation<Error, Product>>(products =>
            {
                return products.First(x => x.Id == id);
            }, erCat =>
            {
                _logger.LogSeqError(erCat);
                return erCat;
            });
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
            var products = await Get(cancellation);
            return products.Match<Validation<Error, Product>>(products =>
            {
                return products.First(x => x.Name == productName);
            }, erCat =>
            {
                _logger.LogSeqError(erCat);
                return erCat;
            });
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
                await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(new[] { product }), _cacheTimeout);
                return ValidationResult.Success!;
            }

            var previousCache = JsonConvert.DeserializeObject<List<Product>>(cachedData, JsonSerializationOptions.RemoveInfiniteLoop)!;
            previousCache.Add(product);

            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(previousCache), _cacheTimeout);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Product '{product.Name}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Update(Product product, CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return new ValidationResult($"Error while Updating Product Id: '{product.Id}', there are no products on Database.");

            var previousCache = JsonConvert.DeserializeObject<List<Product>>(cachedData, JsonSerializationOptions.RemoveInfiniteLoop)!;
            if (previousCache is not { Count: >= 0 })
                return new ValidationResult($"Error while Updating Product, Id: '{product.Id}' was not found on Database.");

            previousCache.RemoveAll(prd => prd.Id == product.Id);
            previousCache.Add(product);
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(previousCache), _cacheTimeout);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating Product '{product.Name}': {ex.Message}");
        }
    }

    // ====== IProductRepositoryCache Methods ======
    public async Task<ValidationResult> InsertAll(FrozenSet<Product> products, CancellationToken cancellation)
    {
        try
        {
            await cache.SetStringAsync(cacheKey, Json.SerializeObjectWithoutReferenceLoop(products), _cacheTimeout);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting All Products: {ex.Message}");
        }
    }

    public async Task<Validation<Error, FrozenSet<Product>>> GetAllFromCacheOrInsertFrom(Func<Task<Validation<Error, FrozenSet<Product>>>> func, CancellationToken cancellation)
    {
        var cacheProducts = await Get(cancellation);
        return await cacheProducts.MatchAsync(async cache =>
        {
            if (cache is not { Count: > 0 })
            {
                var products = await func();
                return await products.MatchAsync<Validation<Error, FrozenSet<Product>>>(async s =>
                {
                    await InsertAll(s.ToFrozenSet(), cancellation);
                    return s.ToFrozenSet();
                }, er => er);
            }
            return cache;
        }, e => e);
    }
    // ====== IProductRepositoryCache Methods ======
}
