using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres;

public sealed class CategoryRepositoryPostgres(ProductDataContext context, ICategoryRepositoryCache categoryRepositoryCache) : ICategoryRepositoryDatabase
{
    private readonly ProductDataContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ICategoryRepositoryCache _cache = categoryRepositoryCache ?? throw new ArgumentNullException(nameof(categoryRepositoryCache));

    public async Task<Results<FrozenSet<Category>, BaseError>> Get(bool byPassCache = false, CancellationToken cancellation = default)
    {
        if (byPassCache)
            return await GetAllDatabase();
        return await _cache.GetAllFromCache(cancellation);
    }

    public async Task<Results<FrozenSet<Category>, BaseError>> Get(CancellationToken cancellation = default)
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

    public async Task<Results<Category, BaseError>> GetById(Guid id, CancellationToken cancellation = default)
    {
        try
        {
            var cacheResult = await _cache.GetById(id, cancellation);
            return await cacheResult.MatchAsync(async categoryCache =>
            {
                if (categoryCache is not null) return categoryCache;
                return await GetFromDatabase();
            }, async e =>
            {
                return await GetFromDatabase();
            });
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Category with id '{id}': {ex.Message}", ex);
        }

        async Task<Results<Category, BaseError>> GetFromDatabase()
        {
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellation);
            if (category is null)
                return (ResultStates.NotFound, new BaseError($"Category '{id}' not found."));
            return category!;
        }
    }

    public async Task<Results<Category, BaseError>> GetByName(string categoryName, CancellationToken cancellation = default)
    {
        try
        {
            var cacheResult = await _cache.GetByName(categoryName, cancellation);
            if (cacheResult.IsSuccess)
            {
                var cacheContent = cacheResult;
                if (cacheContent is not null) return cacheContent!;
                return await GetFromDatabase();
            }
            return await GetFromDatabase();
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Category with name '{categoryName}': {ex.Message}", ex);
        }

        async Task<Results<Category, BaseError>> GetFromDatabase()
        {
            var result = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Name == categoryName, cancellation);
            if (result is null)
                return (ResultStates.NotFound, new BaseError($"Category '{categoryName}' not found."));
            return result!;
        }
    }

    public async Task<ValidationResult> Insert(Category category, CancellationToken cancellation)
    {
        try
        {
            var cacheResult = await _cache.Insert(category, cancellation);
            if (cacheResult != ValidationResult.Success)
            {
                // Log that the cache was not updated
            }

            await _context.Categories.AddAsync(category, cancellation);
            await _context.SaveChangesAsync(true);
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
            var cacheResult = await _cache.Update(category, cancellation);
            if (cacheResult != ValidationResult.Success)
            {
                // Log that the cache was not updated
            }

            _context.Categories.Update(category);
            await _context.SaveChangesAsync(cancellation);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating Category '{category.Name}': {ex.Message}");
        }
    }

    private async Task<Results<FrozenSet<Category>, BaseError>> GetAllDatabase(CancellationToken cancellation = default)
    {
        try
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync(cancellation);
            if (categories is null)
            {
                return FrozenSet<Category>.Empty;
            }
            return categories.ToFrozenSet();
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving all Categories: {ex.Message}", ex);
        }
    }
}
