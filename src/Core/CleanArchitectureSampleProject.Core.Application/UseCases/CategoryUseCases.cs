using CleanArchitectureSampleProject.Core.Application.Inputs.Products;
using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Services;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Core.Application.UseCases;

public interface ICategoryUseCases
{
    Task<Results<FrozenSet<CategoryOutput>, BaseError>> GetCategories(CancellationToken cancellation);
    Task<Results<CategoryOutput, BaseError>> GetCategoryById(Guid categoryId, CancellationToken cancellation);
    Task<Results<CategoryOutput, BaseError>> GetCategoryByName(string categoryName, CancellationToken cancellation);
    Task<Results<CategoryOutput, BaseError>> GetOrCreateCategory(CreateProductInput productInput, CancellationToken cancellation);
    Task<Results<CategoryOutput, BaseError>> GetOrCreateCategoryInternal(UpdateCategoryInput category, CancellationToken cancellation);
    Task<Results<CategoryOutput, BaseError>> CreateCategory(CreateCategoryInput categoryInput, CancellationToken cancellation);
    Task<Results<CategoryOutput, BaseError>> UpdateCategory(UpdateCategoryInput categoryInput, CancellationToken cancellation);
}

public sealed class CategoryUseCases(
    ILogger<CategoryUseCases> logger,
    ICategoryRepository categoryRepository,
    ICreateCategoryService createCategoryService,
    IUpdateCategoryService updateCategoryService) : ICategoryUseCases
{
    private readonly ILogger<CategoryUseCases> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly ICreateCategoryService _createCategoryService = createCategoryService ?? throw new ArgumentNullException(nameof(createCategoryService));
    private readonly IUpdateCategoryService _updateCategoryService = updateCategoryService ?? throw new ArgumentNullException(nameof(updateCategoryService));

    public async Task<Results<FrozenSet<CategoryOutput>, BaseError>> GetCategories(CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName}", nameof(GetCategories));

        var categories = await _categoryRepository.Get(cancellation: cancellation);
        return categories.Match<Results<FrozenSet<CategoryOutput>, BaseError>>(
            categories =>
                categories.Select<Category, CategoryOutput>(cat => cat).ToFrozenSet(),
            err => err);
    }

    public async Task<Results<CategoryOutput, BaseError>> GetCategoryById(Guid categoryId, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryId}", nameof(GetCategoryById), categoryId);

        var category = await _categoryRepository.GetById(categoryId, cancellation: cancellation);
        return category.Match<Results<CategoryOutput, BaseError>>(
            cat => (CategoryOutput)cat,
            err => err);
    }

    public async Task<Results<CategoryOutput, BaseError>> GetCategoryByName(string categoryName, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryName}", nameof(GetCategoryByName), categoryName);

        var category = await _categoryRepository.GetByName(categoryName, cancellation: cancellation);
        if (category.IsSuccess)
            return (CategoryOutput)category.Success!;
        return category.Error!;
    }

    public Task<Results<CategoryOutput, BaseError>> GetOrCreateCategory(CreateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(GetOrCreateCategory), productInput);

        if (productInput?.Category is null)
        {
            var error = new BaseError($"Category {nameof(Category.Id)} or {nameof(Category.Name)} must be informed!");
            return Task.FromResult<Results<CategoryOutput, BaseError>>(error);
        }

        if (productInput.Category.Id is null)
            return CreateCategory(new CreateCategoryInput { Name = productInput.Category.Name }, cancellation);
        return GetCategoryById(productInput.Category.Id.Value, cancellation);
    }

    public Task<Results<CategoryOutput, BaseError>> GetOrCreateCategoryInternal(UpdateCategoryInput category, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryInput}", nameof(GetOrCreateCategoryInternal), category);

        if (category.Id is null)
            return CreateCategory(new CreateCategoryInput { Name = category.Name }, cancellation);
        return GetCategoryById(category.Id.Value, cancellation);
    }

    public async Task<Results<CategoryOutput, BaseError>> CreateCategory(CreateCategoryInput categoryInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryInput}", nameof(CreateCategory), categoryInput);

        var category = categoryInput.ToCategory();
        var result = await _createCategoryService.Execute(category, cancellation);
        if (result.IsFail) return (result.State, result.Error!);
        return (CategoryOutput)result.Success!;
    }

    public async Task<Results<CategoryOutput, BaseError>> UpdateCategory(UpdateCategoryInput categoryInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryInput}", nameof(UpdateCategory), categoryInput);

        var category = categoryInput.ToCategory();
        var result = await _updateCategoryService.Execute(category, cancellation);
        if (result.IsFail) return result.Error;
        return (CategoryOutput)result.Success;
    }
}
