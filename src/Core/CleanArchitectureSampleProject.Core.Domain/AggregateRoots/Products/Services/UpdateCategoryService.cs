using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Services;

public interface IUpdateCategoryService
{
    Task<Results<Category, BaseError>> Execute(Category categoryInput, CancellationToken cancellationToken);
}

internal class UpdateCategoryService : IUpdateCategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Results<Category, BaseError>> Execute(Category categoryInput, CancellationToken cancellationToken)
    {
        if (categoryInput.Id == Guid.Empty || string.IsNullOrWhiteSpace(categoryInput.Name))
        {
            var error = new BaseError($"Category {nameof(Category.Id)} and {nameof(Category.Name)} must be informed!");
            return error;
        }

        var result = await _categoryRepository.GetById(categoryInput.Id, cancellationToken);
        return await result.MatchAsync(
            async oldCategory =>
                await UpdateCatetory(categoryInput, oldCategory, cancellationToken),
            error =>
                error);
    }

    private async Task<Results<Category, BaseError>> UpdateCatetory(Category categoryInput, Category oldCategory, CancellationToken cancellation)
    {
        var categoryGetResult = await _categoryRepository.GetByName(categoryInput.Name);
        if (categoryGetResult.State is ResultStates.Error) return categoryGetResult.Error!;
        if (categoryGetResult.IsSuccess && categoryInput.Id != categoryGetResult.ToSuccess().Id) return new BaseError($"Category '{categoryInput.Name}' already exists.");

        oldCategory.Update(categoryInput);
        var result = await _categoryRepository.Update(oldCategory, cancellation);
        if (result != ValidationResult.Success)
            return new BaseError(result.ErrorMessage!);
        return oldCategory;
    }
}
