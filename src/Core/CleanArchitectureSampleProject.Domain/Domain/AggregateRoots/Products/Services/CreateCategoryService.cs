using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Domain.Domain.AggregateRoots.Products.Services;

public interface ICreateCategoryService
{
    Task<Validation<Error, Category>> Execute(Category category, CancellationToken cancellationToken);
}

public sealed class CreateCategoryService : ICreateCategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Validation<Error, Category>> Execute(Category categoryInput, CancellationToken cancellationToken)
    {
        if (categoryInput is null) return Error.New("Category must not be null.");

        Validation<Error, Category> categoryResult = categoryInput.Validate();
        if (categoryResult.IsFail) return categoryResult.ToError();

        var categoryGetResult = await _categoryRepository.GetByName(categoryInput.Name);
        if (categoryGetResult.IsSuccess is false) return categoryGetResult.Error!;
        if (categoryGetResult.Result is not null) return Error.New($"Category '{categoryInput.Name}' already exists.");

        categoryInput.Create();
        var creationResult = await _categoryRepository.Insert(categoryInput, cancellationToken);
        if (creationResult != ValidationResult.Success) return Error.New(creationResult.ErrorMessage!);

        return categoryInput;
    }
}
