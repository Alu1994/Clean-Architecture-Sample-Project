using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Services;

public interface ICreateCategoryService
{
    Task<Results<Category, BaseError>> Execute(Category categoryInput, CancellationToken cancellationToken);
}

public sealed class CreateCategoryService : ICreateCategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Results<Category, BaseError>> Execute(Category categoryInput, CancellationToken cancellationToken)
    {
        if (categoryInput is null) return new BaseError("Category must not be null.");

        var categoryGetResult = await _categoryRepository.GetByName(categoryInput.Name);
        if (categoryGetResult.State is ResultStates.Error) return categoryGetResult.Error!;
        if (categoryGetResult.IsSuccess) return new BaseError($"Category '{categoryInput.Name}' already exists.");

        categoryInput.Create();
        var creationResult = await _categoryRepository.Insert(categoryInput, cancellationToken);
        if (creationResult != ValidationResult.Success) return new BaseError(creationResult.ErrorMessage!);

        return categoryInput;
    }
}
