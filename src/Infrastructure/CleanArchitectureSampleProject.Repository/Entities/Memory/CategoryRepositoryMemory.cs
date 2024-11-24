using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Memory;

public sealed class CategoryRepositoryMemory : ICategoryRepository
{
    private List<Category> _categories = [];

    public async Task<Results<FrozenSet<Category>, BaseError>> Get(CancellationToken cancellation = default)
    {
        try 
        {
            return await Task.FromResult(_categories.ToFrozenSet());
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
            return await Task.FromResult(_categories.First(x => x.Id == id));
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
            var result = _categories.FirstOrDefault(x => x.Name == categoryName);
            if(result is null) return await Task.FromResult(ResultStates.NotFound);
            return await Task.FromResult(result);
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
            _categories.Add(category);
            return await Task.FromResult(ValidationResult.Success!);
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
            var memoryCategory = await GetById(category.Id, cancellation: cancellation);
            memoryCategory.Match<Results<Category, BaseError>>(cat => { cat.Name = category.Name; return cat; }, _ => _);
            return await Task.FromResult(ValidationResult.Success!);
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating Category '{category.Name}': {ex.Message}");
        }
    }
}
