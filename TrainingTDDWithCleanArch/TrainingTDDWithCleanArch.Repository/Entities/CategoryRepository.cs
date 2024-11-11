using System.Collections.Frozen;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products.Entities;
using TrainingTDDWithCleanArch.Domain.Interfaces;

namespace TrainingTDDWithCleanArch.Repository.Entities;

public sealed class CategoryRepository : ICategoryRepository
{
    private List<Category> _categories = [];

    public async Task<Validation<Error, FrozenSet<Category>>> Get(CancellationToken cancellation)
    {
        try
        {
            return await Task.FromResult(_categories.ToFrozenSet());
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving all Products: {ex.Message}");
        }
    }

    public async Task<Validation<Error, Category>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            return await Task.FromResult(_categories.First(x => x.Id == id));
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Category with id '{id}': {ex.Message}");
        }
    }

    public async Task<Validation<Error, Category>> GetByName(string categoryName, CancellationToken cancellation)
    {
        try
        {
            return await Task.FromResult(_categories.First(x => x.Name == categoryName));
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Category with name '{categoryName}': {ex.Message}");
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
        throw new NotImplementedException();
    }
}
