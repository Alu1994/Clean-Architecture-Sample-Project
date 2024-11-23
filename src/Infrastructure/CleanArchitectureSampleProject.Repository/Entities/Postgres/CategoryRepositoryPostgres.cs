using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres;

public sealed class CategoryRepositoryPostgres(ProductDataContext context, ICategoryRepositoryCache categoryRepositoryCache) : ICategoryRepositoryDatabase
{
    private readonly ProductDataContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ICategoryRepositoryCache _cache = categoryRepositoryCache ?? throw new ArgumentNullException(nameof(categoryRepositoryCache));

    public async Task<Validation<Error, FrozenSet<Category>>> Get(bool byPassCache = false, CancellationToken cancellation = default)
    {
        if (byPassCache)
            return await GetAllDatabase();
        return await _cache.GetAllFromCache(cancellation);
    }

    public async Task<Validation<Error, FrozenSet<Category>>> Get(CancellationToken cancellation = default)
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

    public async Task<Validation<Error, Category>> GetById(Guid id, CancellationToken cancellation = default)
    {
        try
        {
            var cacheResult = await _cache.GetById(id, cancellation);
            return await cacheResult.MatchAsync<Validation<Error, Category>>(async categoryCache =>
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
            return Error.New($"Error while retrieving Category with id '{id}': {ex.Message}", ex);
        }

        Task<Category> GetFromDatabase()
        {
            var category = _context.Categories.AsNoTracking().FirstAsync(x => x.Id == id);
            return category!;
        }
    }

    public async Task<Validation<Error, Category>> GetByName(string categoryName, CancellationToken cancellation = default)
    {
        try
        {
            var cacheResult = await _cache.GetByName(categoryName, cancellation);
            return await cacheResult.MatchAsync<Validation<Error, Category>>(async categoryCache =>
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
            return Error.New($"Error while retrieving Category with name '{categoryName}': {ex.Message}", ex);
        }

        Task<Category> GetFromDatabase()
        {
            var category = _context.Categories.AsNoTracking().FirstAsync(x => x.Name == categoryName);
            return category!;
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
            await _context.SaveChangesAsync();
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating Category '{category.Name}': {ex.Message}");
        }
    }

    private async Task<Validation<Error, FrozenSet<Category>>> GetAllDatabase(CancellationToken cancellation = default)
    {
        try
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            if (categories is null)
            {
                return Enumerable.Empty<Category>().ToFrozenSet();
            }
            return categories.ToFrozenSet();
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving all Categories: {ex.Message}", ex);
        }
    }
}
