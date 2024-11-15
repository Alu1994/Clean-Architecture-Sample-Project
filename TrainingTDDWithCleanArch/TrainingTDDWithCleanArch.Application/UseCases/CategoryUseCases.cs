using System.Collections.Frozen;
using TrainingTDDWithCleanArch.Application.Inputs;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products.Entities;
using TrainingTDDWithCleanArch.Domain.Interfaces;

namespace TrainingTDDWithCleanArch.Application.UseCases;

public interface ICategoryUseCases
{
    Task<Validation<Error, FrozenSet<Category>>> GetCategories(CancellationToken cancellation);
    Task<Validation<Error, Category>> GetCategoryById(Guid categoryId, CancellationToken cancellation);
    Task<Validation<Error, Category>> GetCategoryByName(string categoryName, CancellationToken cancellation);
    Task<Validation<Error, Category>> CreateCategory(CreateCategoryInput categoryInput, CancellationToken cancellation);
    Task<Validation<Error, Category>> GetOrCreateCategory(CreateProductInput productInput, CancellationToken cancellation);
}

public sealed class CategoryUseCases(ILogger<CategoryUseCases> logger, ICategoryRepository categoryRepository) : ICategoryUseCases
{
    private readonly ILogger<CategoryUseCases> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));

    public async Task<Validation<Error, FrozenSet<Category>>> GetCategories(CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName}", nameof(GetCategories));
        return await _categoryRepository.Get(cancellation);
    }

    public async Task<Validation<Error, Category>> GetCategoryById(Guid categoryId, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryId}", nameof(GetCategoryById), categoryId);
        return await _categoryRepository.GetById(categoryId, cancellation);
    }

    public async Task<Validation<Error, Category>> GetCategoryByName(string categoryName, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryName}", nameof(GetCategoryByName), categoryName);
        return await _categoryRepository.GetByName(categoryName, cancellation);
    }

    public async Task<Validation<Error, Category>> CreateCategory(CreateCategoryInput categoryInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {CategoryInput}", nameof(CreateCategory), categoryInput);

        return await categoryInput.ToCategory().MatchAsync<Validation<Error, Category>>(async category => {
            var repoResult = await _categoryRepository.Insert(category, cancellation);
            if (repoResult == ValidationResult.Success)
                return category;
            return Error.New(repoResult.ErrorMessage!);
        }, error => error);
    }

    public async Task<Validation<Error, Category>> GetOrCreateCategory(CreateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(GetOrCreateCategory), productInput);

        if (productInput?.Category is null) return Error.New($"Category {nameof(Category.Id)} or {nameof(Category.Name)} must be informed!");

        if (productInput.Category.Id is null)
            return await CreateCategory(productInput.Category, cancellation);
        return await GetCategoryById(productInput.Category.Id.Value, cancellation);
    }
}
