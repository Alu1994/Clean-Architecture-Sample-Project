using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Memory;

public sealed class CategoryRepositoryMemory : ICategoryRepository
{
    private List<Category> _categories = [];

    public async Task<Validation<Error, FrozenSet<Category>>> Get(CancellationToken cancellation = default)
    {
        try
        {
            return await Task.FromResult(_categories.ToFrozenSet());
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving all Categories: {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Category>> GetById(Guid id, CancellationToken cancellation = default)
    {
        try
        {
            return await Task.FromResult(_categories.First(x => x.Id == id));
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Category with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Results<Category, Error>> GetByName(string categoryName, CancellationToken cancellation = default)
    {
        try
        {
            var result = _categories.FirstOrDefault(x => x.Name == categoryName);
            if(result is null) return await Task.FromResult(ResultStates.NotFound);
            return await Task.FromResult(result);
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
            memoryCategory.Match(cat => cat.Name = category.Name, _ => { });
            return await Task.FromResult(ValidationResult.Success!);
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating Category '{category.Name}': {ex.Message}");
        }
    }
}
