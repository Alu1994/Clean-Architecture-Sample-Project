using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres;

public sealed class CategoryRepositoryPostgres(ProductDataContext context, ICategoryRepositoryCache categoryRepositoryCache) : ICategoryRepository
{
    private readonly ProductDataContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ICategoryRepositoryCache _cache = categoryRepositoryCache ?? throw new ArgumentNullException(nameof(categoryRepositoryCache));

    public async Task<Validation<Error, FrozenSet<Category>>> Get(CancellationToken cancellation)
    {
        return await _cache.GetAllFromCacheOrInsertFrom(async () =>
        {
            try
            {
                var categories = await _context.Categories.AsNoTracking().ToListAsync();
                if (categories == null)
                {
                    return Enumerable.Empty<Category>().ToFrozenSet();
                }
                return categories.ToFrozenSet();
            }
            catch (Exception ex)
            {
                return Error.New($"Error while retrieving all Categories: {ex.Message}", ex);
            }
        }, cancellation);
    }

    public async Task<Validation<Error, Category>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            var category = await _context.Categories.AsNoTracking().FirstAsync(x => x.Id == id);
            return category!;
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
            var category = await _context.Categories.AsNoTracking().FirstAsync(x => x.Name == categoryName);
            return category!;
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
}
