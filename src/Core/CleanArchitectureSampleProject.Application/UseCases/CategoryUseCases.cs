using CleanArchitectureSampleProject.Application.Inputs;
using CleanArchitectureSampleProject.Application.Outputs;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Domain.AggregateRoots.Products.Services;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Application.UseCases;

public interface ICategoryUseCases
{
    Task<Validation<Error, FrozenSet<CategoryOutput>>> GetCategories(CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> GetCategoryById(Guid categoryId, CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> GetCategoryByName(string categoryName, CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> GetOrCreateCategory(CreateProductInput productInput, CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> GetOrCreateCategoryInternal(CategoryInput category, CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> CreateCategory(CategoryInput categoryInput, CancellationToken cancellation);
    Task<Validation<Error, CategoryOutput>> UpdateCategory(CategoryInput categoryInput, CancellationToken cancellation);
}

public sealed class CategoryUseCases(
    ILogger<CategoryUseCases> logger, 
    ICategoryRepository categoryRepository,
    ICreateCategoryService createCategoryService) : ICategoryUseCases
{
    private readonly ILogger<CategoryUseCases> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly ICreateCategoryService _createCategoryService = createCategoryService ?? throw new ArgumentNullException(nameof(createCategoryService));

    public async Task<Validation<Error, FrozenSet<CategoryOutput>>> GetCategories(CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName}", nameof(GetCategories));
        var categories = await _categoryRepository.Get(cancellation: cancellation);
        return categories.Match<Validation<Error, FrozenSet<CategoryOutput>>>(
            categories =>
                categories.Select<Category, CategoryOutput>(cat => cat).ToFrozenSet(),
            err => 
                err);
    }

    public async Task<Validation<Error, CategoryOutput>> GetCategoryById(Guid categoryId, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryId}", nameof(GetCategoryById), categoryId);
        var category = await _categoryRepository.GetById(categoryId, cancellation: cancellation);
        return category.Match<Validation<Error, CategoryOutput>>(
            cat =>
                (CategoryOutput)cat, 
            err => 
                err);
    }

    public async Task<Validation<Error, CategoryOutput>> GetCategoryByName(string categoryName, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryName}", nameof(GetCategoryByName), categoryName);
        var category = await _categoryRepository.GetByName(categoryName, cancellation: cancellation);
        if(category.IsSuccess)
        {
            return (CategoryOutput)category.Result!;
        }
        return category.Error!;
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
        _logger.LogInformation("Logging {MethodName} with {CategoryInput}", nameof(GetOrCreateCategoryInternal), category);
        if (category.Id is null)
            return CreateCategory(category, cancellation);
        return GetCategoryById(category.Id.Value, cancellation);
    }

    public async Task<Validation<Error, CategoryOutput>> CreateCategory(CategoryInput categoryInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryInput}", nameof(CreateCategory), categoryInput);

        var category = categoryInput.ToCategory2();
        var res = await _createCategoryService.Execute(category, cancellation);

        if (res.IsFail) return res.ToError();

        return (CategoryOutput)res.ToSuccess();

        //return await categoryInput.ToCategory().MatchAsync<Validation<Error, CategoryOutput>>(
        //    async category =>
        //    {
        //        var repoResult = await _categoryRepository.Insert(category, cancellation);
        //        if (repoResult != ValidationResult.Success) return Error.New(repoResult.ErrorMessage!);
        //        return (CategoryOutput)category;                
        //    }, 
        //    error => 
        //        error);
    }

    public async Task<Validation<Error, CategoryOutput>> UpdateCategory(CategoryInput categoryInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryInput}", nameof(UpdateCategory), categoryInput);

        if (categoryInput.Id is null || string.IsNullOrWhiteSpace(categoryInput.CategoryName))
        {
            var error = Error.New($"Category {nameof(Category.Id)} and {nameof(Category.Name)} must be informed!");
            return error;
        }

        var result = await GetCategoryById(categoryInput.Id.Value, cancellation);
        return await result.MatchAsync(
            async oldCategory =>
                await UpdateCatetory(categoryInput, oldCategory, cancellation), 
            error => 
                error);
    }

    private Task<Validation<Error, CategoryOutput>> UpdateCatetory(CategoryInput categoryInput, CategoryOutput categoryOutput, CancellationToken cancellation)
    {
        var categoryResult = categoryOutput.ToCategory();
        return categoryResult.MatchAsync(
            async oldCategory => 
                await Update(categoryInput, oldCategory, cancellation), 
            error => 
                error);

        async Task<Validation<Error, CategoryOutput>> Update(CategoryInput categoryInput, Category oldCategory, CancellationToken cancellation)
        {
            var validationResult = categoryInput.ToCategory();
            var updateCategoryResult = validationResult.Match<Validation<Error, Category>>(
                newCategory =>
                    oldCategory.Update(newCategory),
                updateError =>
                    updateError);

            if (updateCategoryResult.IsFail) return (Seq<Error>)updateCategoryResult;
            var result = await _categoryRepository.Update(oldCategory, cancellation);
            if (result != ValidationResult.Success) return Error.New(result.ErrorMessage!);
            return (CategoryOutput)oldCategory;
        }
    }
}
