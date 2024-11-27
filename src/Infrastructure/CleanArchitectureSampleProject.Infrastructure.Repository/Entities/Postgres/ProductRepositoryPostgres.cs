using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres;

public sealed class ProductRepositoryPostgres(ProductDataContext context, IProductRepositoryCache productRepositoryCache) : IProductRepositoryDatabase
{
    private readonly ProductDataContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IProductRepositoryCache _cache = productRepositoryCache ?? throw new ArgumentNullException(nameof(productRepositoryCache));

    public async Task<Results<FrozenSet<Product>, BaseError>> Get(bool byPassCache = false, CancellationToken cancellation = default)
    {
        if (byPassCache)
            return await GetAllDatabase();
        return await _cache.GetAllFromCache(cancellation);
    }

    public async Task<Results<FrozenSet<Product>, BaseError>> Get(CancellationToken cancellation = default)
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

    public async Task<Results<Product, BaseError>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            var cacheResult = await _cache.GetById(id, cancellation);
            return await cacheResult.MatchAsync(async productCache =>
            {
                if (productCache is not null) return productCache;
                return await GetFromDatabase();
            }, async e =>
            {
                return await GetFromDatabase();
            });
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Product with id '{id}': {ex.Message}", ex);
        }

        async Task<Results<Product, BaseError>> GetFromDatabase()
        {
            var product = await _context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellation);
            if (product is null) return (ResultStates.NotFound, new BaseError($"Product '{id}' not found"));
            return product;
        }
    }

    public async Task<Results<Product, BaseError>> GetByIdOrDefault(Guid id, CancellationToken cancellation)
    {
        try
        {
            var cacheResult = await _cache.GetById(id, cancellation);
            return await cacheResult.MatchAsync(async productCache =>
            {
                if (productCache is not null) return productCache;
                return await GetFromDatabase();
            }, async e =>
            {
                return await GetFromDatabase();
            });
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Product with id '{id}': {ex.Message}", ex);
        }

        async Task<Results<Product, BaseError>> GetFromDatabase()
        {
            var product = await _context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellation);
            if (product is null) return (ResultStates.NotFound, new BaseError($"Product '{id}' not found"));
            return product;
        }
    }

    public async Task<Results<Product, BaseError>> GetByName(string productName, CancellationToken cancellation)
    {
        try
        {
            var cacheResult = await _cache.GetByName(productName, cancellation);
            if (cacheResult.IsSuccess)
            {
                if (cacheResult.Success is not null) return cacheResult.Success!;
                return await GetFromDatabase();
            }
            return await GetFromDatabase();
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Product with name '{productName}': {ex.Message}", ex);
        }

        async Task<Results<Product, BaseError>> GetFromDatabase()
        {
            var product = await _context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Name == productName, cancellation);
            if (product is null) return (ResultStates.NotFound, new BaseError($"Product '{productName}' not found"));
            return product;
        }
    }

    public async Task<ValidationResult> Insert(Product product, CancellationToken cancellation)
    {
        try
        {
            product.Category = null;

            var cacheResult = await _cache.Insert(product, cancellation);
            if (cacheResult != ValidationResult.Success)
            {
                // Log that the cache was not updated
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
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
            product.Category = null;

            var cacheResult = await _cache.Update(product, cancellation);
            if (cacheResult != ValidationResult.Success)
            {
                // Log that the cache was not updated
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Product '{product.Name}': {ex.Message}");
        }
    }

    private async Task<Results<FrozenSet<Product>, BaseError>> GetAllDatabase(CancellationToken cancellation = default)
    {
        try
        {
            var products = await _context.Products.Include(x => x.Category).AsNoTracking().ToListAsync(cancellation);
            if (products == null)
            {
                return FrozenSet<Product>.Empty;
            }
            return products.ToFrozenSet();
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving all Products: {ex.Message}", ex);
        }
    }
}
