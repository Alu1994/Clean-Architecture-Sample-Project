using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Services;

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
        var validationResult = categoryInput.Validate();
        return await validationResult.MatchAsync<Results<Category, BaseError>>(
            async newCategory =>
            {
                oldCategory.Update(newCategory);
                var result = await _categoryRepository.Update(oldCategory, cancellation);
                if (result != ValidationResult.Success)
                    return new BaseError(result.ErrorMessage!);
                return oldCategory;
            },
            updateError => updateError);
    }
}
