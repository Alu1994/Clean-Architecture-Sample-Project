using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Services;

public interface ICategoryGetOrCreateService
{
    Task<Results<Category, ErrorList>> Execute(Category category, CancellationToken cancellationToken);
}

public sealed class CategoryGetOrCreateService : ICategoryGetOrCreateService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGetOrCreateCategoryValidator _validator;

    public CategoryGetOrCreateService(ICategoryRepository categoryRepository, IGetOrCreateCategoryValidator validator)
    {
        _categoryRepository = categoryRepository;
        _validator = validator;
    }

    public async Task<Results<Category, ErrorList>> Execute(Category categoryInput, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(categoryInput);
        if (validationResult.IsValid is false) return new ErrorList(validationResult);

        if (categoryInput is null) return new ErrorList("Category must not be null.");
        var categoryResult = categoryInput.ValidateGetOrCreate();
        if (categoryResult.IsFail) return new ErrorList(categoryResult.Error);

        if (categoryInput.Id != Guid.Empty)
        {
            var categoryGetResult = await _categoryRepository.GetById(categoryInput.Id);
            if (categoryGetResult.IsFail) return new ErrorList(categoryGetResult.Error);
            return categoryGetResult.ToSuccess();
        }

        if (string.IsNullOrWhiteSpace(categoryInput.Name) is false)
        {
            var categoryGetResult = await _categoryRepository.GetByName(categoryInput.Name);
            if (categoryGetResult.State is ResultStates.Error) return new ErrorList(categoryGetResult.Error);
            if (categoryGetResult.State is ResultStates.Success) return new ErrorList($"Category '{categoryInput.Name}' already exists.");
        }

        var creationResult = await _categoryRepository.Insert(categoryInput, cancellationToken);
        if (creationResult != ValidationResult.Success) return new ErrorList(creationResult.ErrorMessage!);

        return categoryInput;
    }
}
