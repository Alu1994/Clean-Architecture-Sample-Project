using Microsoft.Extensions.Caching.Distributed;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Cache;

public sealed class ProductRepositoryCache(ILogger<ProductRepositoryCache> logger, ICategoryRepository categoryRepository, IDistributedCache cache) : IProductRepositoryCache
{
    private readonly ILogger<ProductRepositoryCache> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    private const string cacheKey = "Products";
    private readonly DistributedCacheEntryOptions _cacheTimeout = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiry
    };

    public async Task<Results<FrozenSet<Product>, BaseError>> Get(CancellationToken cancellation)
    {
        try
        {
            string? cachedData = await _cache.GetStringAsync(cacheKey, cancellation);
            if (cachedData is null)
                return Enumerable.Empty<Product>().ToFrozenSet();

            var products = JsonConvert.DeserializeObject<List<Product>>(cachedData, JsonSerializationOptions.RemoveInfiniteLoop)!.ToFrozenSet();
            foreach (var product in products)
            {
                var categoryResult = await _categoryRepository.GetById(product.CategoryId, cancellation: cancellation);
                _ = categoryResult.Match<Results<Product, BaseError>>(category =>
                {
                    return product.WithCategory(category);
                }, erCat =>
                {
                    _logger.LogBaseError(erCat);
                    return erCat;
                });
            }
            return products;
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving all Products: {ex.Message}", ex);
        }
    }

    public async Task<Results<Product, BaseError>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            var products = await Get(cancellation);
            return products.Match<Results<Product, BaseError>>(products =>
            {
                return products.First(x => x.Id == id);
            }, erCat =>
            {
                _logger.LogBaseError(erCat);
                return erCat;
            });
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Product with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Results<Product, BaseError>> GetByIdOrDefault(Guid id, CancellationToken cancellation)
    {
        try
        {
            var products = await Get(cancellation);
            return products.Match<Results<Product, BaseError>>(products =>
            {
                return products.FirstOrDefault(x => x.Id == id);
            }, erCat =>
            {
                _logger.LogBaseError(erCat);
                return erCat;
            });
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Product with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Results<Product, BaseError>> GetByName(string productName, CancellationToken cancellation)
    {
        try
        {
            var products = await Get(cancellation);
            if (!products.IsSuccess)
            {
                _logger.LogBaseError(products.Error);
                return products.Error;
            }

            var result = products.ToSuccess().FirstOrDefault(x => x.Name == productName);
            if (result is not null) return result;
            return (ResultStates.NotFound, new BaseError($"Product '{productName}' not found."));
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Product with name '{productName}': {ex.Message}", ex);
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

    public async Task<Results<FrozenSet<Product>, BaseError>> GetAllFromCache(CancellationToken cancellation)
    {
        var cacheProducts = await Get(cancellation);
        return cacheProducts.Match<Results<FrozenSet<Product>, BaseError>>(cache => cache, e => e);
    }
    // ====== IProductRepositoryCache Methods ======
}
