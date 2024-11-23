using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres;

public sealed class ProductRepositoryPostgres(ProductDataContext context, IProductRepositoryCache productRepositoryCache) : IProductRepository
{
    private readonly ProductDataContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IProductRepositoryCache _cache = productRepositoryCache ?? throw new ArgumentNullException(nameof(productRepositoryCache));

    public async Task<Validation<Error, FrozenSet<Product>>> Get(CancellationToken cancellation)
    {
        return await _cache.GetAllFromCacheOrInsertFrom(async () =>
        {
            try
            {
                var products = await _context.Products.Include(x => x.Category).AsNoTracking().ToListAsync(cancellation);
                if (products == null)
                {
                    return Enumerable.Empty<Product>().ToFrozenSet();
                }
                return products.ToFrozenSet();
            }
            catch (Exception ex)
            {
                return Error.New($"Error while retrieving all Products: {ex.Message}", ex);
            }
        }, cancellation);
    }

    public async Task<Validation<Error, Product>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            var cacheResult = await _cache.GetById(id, cancellation);
            return await cacheResult.MatchAsync<Validation<Error, Product>>(async productCache =>
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
            return Error.New($"Error while retrieving Product with id '{id}': {ex.Message}", ex);
        }

        Task<Product> GetFromDatabase()
        {
            var product = _context.Products.Include(x => x.Category).AsNoTracking().FirstAsync(x => x.Id == id, cancellation);
            return product!;
        }
    }

    public async Task<Validation<Error, Product>> GetByName(string productName, CancellationToken cancellation)
    {
        try
        {
            var cacheResult = await _cache.GetByName(productName, cancellation);
            return await cacheResult.MatchAsync<Validation<Error, Product>>(async productCache =>
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
            return Error.New($"Error while retrieving Product with name '{productName}': {ex.Message}", ex);
        }

        Task<Product> GetFromDatabase()
        {
            var product = _context.Products.Include(x => x.Category).AsNoTracking().FirstAsync(x => x.Name == productName, cancellation);
            return product!;
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
}
