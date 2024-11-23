using System.Collections.Frozen;
using CleanArchitectureSampleProject.Application.Inputs;
using CleanArchitectureSampleProject.Application.Outputs;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Application.UseCases;

public interface ICategoryUseCases
{
    Task<Validation<Error, FrozenSet<CategoryOutput>>> GetCategories(CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> GetCategoryById(Guid categoryId, CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> GetCategoryByName(string categoryName, CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> CreateCategory(CategoryInput categoryInput, CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> UpdateCategory(CategoryInput categoryInput, CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> GetOrCreateCategory(CreateProductInput productInput, CancellationToken cancellation);

    Task<Validation<Error, CategoryOutput>> GetOrCreateCategoryInternal(CategoryInput category, CancellationToken cancellation);
}

public sealed class CategoryUseCases(ILogger<CategoryUseCases> logger, ICategoryRepository categoryRepository) : ICategoryUseCases
{
    private readonly ILogger<CategoryUseCases> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));

    public async Task<Validation<Error, FrozenSet<CategoryOutput>>> GetCategories(CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName}", nameof(GetCategories));
        var categories = await _categoryRepository.Get(cancellation);
        return categories.Match<Validation<Error, FrozenSet<CategoryOutput>>>(categories =>
            categories.Select<Category, CategoryOutput>(cat => cat).ToFrozenSet(),
            err => err);
    }

    public async Task<Validation<Error, CategoryOutput>> GetCategoryById(Guid categoryId, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryId}", nameof(GetCategoryById), categoryId);
        var category = await _categoryRepository.GetById(categoryId, cancellation);
        return category.Match<Validation<Error, CategoryOutput>>(cat =>
            (CategoryOutput)cat, 
            err => err);
    }

    public async Task<Validation<Error, CategoryOutput>> GetCategoryByName(string categoryName, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryName}", nameof(GetCategoryByName), categoryName);
        var category = await _categoryRepository.GetByName(categoryName, cancellation);
        return category.Match<Validation<Error, CategoryOutput>>(cat =>
            (CategoryOutput)cat,
            err => err);
    }

    public async Task<Validation<Error, CategoryOutput>> CreateCategory(CategoryInput categoryInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryInput}", nameof(CreateCategory), categoryInput);

        return await categoryInput.ToCategory().MatchAsync<Validation<Error, CategoryOutput>>(async category =>
        {
            var repoResult = await _categoryRepository.Insert(category, cancellation);
            if (repoResult == ValidationResult.Success)
                return (CategoryOutput)category;
            return Error.New(repoResult.ErrorMessage!);
        }, error => error);
    }

    public async Task<Validation<Error, CategoryOutput>> UpdateCategory(CategoryInput categoryInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryInput}", nameof(UpdateCategory), categoryInput);

        var result = await GetCategoryById(categoryInput.Id.Value, cancellation);
        return await result.MatchAsync(async oldCategory =>
            await UpdateCatetory(categoryInput, oldCategory, cancellation)
            , error => error);
    }

    public Task<Validation<Error, CategoryOutput>> GetOrCreateCategory(CreateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(GetOrCreateCategory), productInput);

        if (productInput?.Category is null)
        {
            var error = Error.New($"Category {nameof(Category.Id)} or {nameof(Category.Name)} must be informed!");
            return Task.FromResult<Validation<Error, CategoryOutput>>(error);
        }

        if (productInput.Category.Id is null)
            return CreateCategory(productInput.Category, cancellation);
        return GetCategoryById(productInput.Category.Id.Value, cancellation);
    }

    public Task<Validation<Error, CategoryOutput>> GetOrCreateCategoryInternal(CategoryInput category, CancellationToken cancellation)
    {
        if (category.Id is null)
            return CreateCategory(category, cancellation);
        return GetCategoryById(category.Id.Value, cancellation);
    }

    private Task<Validation<Error, CategoryOutput>> UpdateCatetory(CategoryInput categoryInput, CategoryOutput getCategoryById, CancellationToken cancellation)
    {
        var categoryResult = getCategoryById.ToCategory();
        return categoryResult.MatchAsync<Validation<Error, CategoryOutput>>(async category =>
        {
            var categoryConvertResult = categoryInput.ToCategory().Match<Validation<Error, Category>>(j => category.Update(j), ie => ie);
            if (categoryConvertResult.IsFail) return (Seq<Error>)categoryConvertResult;

            var repoResult = await _categoryRepository.Update(category, cancellation);
            if (repoResult == ValidationResult.Success)
                return (CategoryOutput)category;
            return Error.New(repoResult.ErrorMessage!);
        }, getCatErr => getCatErr);
    }
}
