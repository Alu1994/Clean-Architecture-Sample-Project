using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Domain.Domain.AggregateRoots.Products.Services;

public interface ICategoryGetOrCreateService
{
    Task<Results<Category, BaseError>> Execute(Category category, CancellationToken cancellationToken);
}

public sealed class CategoryGetOrCreateService : ICategoryGetOrCreateService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryGetOrCreateService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Results<Category, BaseError>> Execute(Category categoryInput, CancellationToken cancellationToken)
    {
        if (categoryInput is null) return new BaseError("Category must not be null.");
        var categoryResult = categoryInput.ValidateGetOrCreate();
        if(categoryResult.IsFail) return categoryResult;

        if (categoryInput.Id != Guid.Empty)
        {
            var categoryGetResult = await _categoryRepository.GetById(categoryInput.Id);
            return categoryGetResult;
        }

        var creationResult = await _categoryRepository.Insert(categoryInput, cancellationToken);
        if (creationResult != ValidationResult.Success) return new BaseError(creationResult.ErrorMessage!);

        return categoryInput;
    }
}
