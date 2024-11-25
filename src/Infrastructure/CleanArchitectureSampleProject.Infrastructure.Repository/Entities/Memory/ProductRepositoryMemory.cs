using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Memory;

public sealed class ProductRepositoryMemory(ILogger<ProductRepositoryMemory> logger, ICategoryRepository categoryRepository) : IProductRepository
{
    private readonly ILogger<ProductRepositoryMemory> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));

    //private List<Product> _products = [ Product.Create("Product", "Product", 0, 0, Domain.AggregateRoots.Products.Entities.Category.Create(Guid.NewGuid(), "Category").SuccessToSeq().First()).SuccessToSeq().First() ];
    private List<Product> _products = [];

    public async Task<Results<FrozenSet<Product>, BaseError>> Get(CancellationToken cancellation)
    {
        try
        {
            var products = await Task.FromResult(_products.ToFrozenSet());
            foreach (var product in products)
            {
                var categoryResult = await _categoryRepository.GetById(product.Category.Id, cancellation: cancellation);
                _ = categoryResult.Match<Results<Product, BaseError>>(category =>
                {
                    return product.WithCategory(category);
                }, erCat =>
                {
                    _logger.LogBaseError(erCat);
                    return erCat;
                });
            }
            return products;
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving all Products: {ex.Message}", ex);
        }
    }

    public async Task<Results<Product, BaseError>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            return await Task.FromResult(_products.First(x => x.Id == id));
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Product with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Results<Product, BaseError>> GetByIdOrDefault(Guid id, CancellationToken cancellation)
    {
        try
        {
            return await Task.FromResult(_products.FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Product with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Results<Product, BaseError>> GetByName(string productName, CancellationToken cancellation)
    {
        try
        {
            var result = _products.FirstOrDefault(x => x.Name == productName);
            if(result is null) return await Task.FromResult((ResultStates.NotFound, new BaseError($"Product '{productName}' not found.")));
            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while retrieving Product with name '{productName}': {ex.Message}", ex);
        }
    }

    public async Task<ValidationResult> Insert(Product product, CancellationToken cancellation)
    {
        try
        {
            _products.Add(product);
            return await Task.FromResult(ValidationResult.Success!);
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Product '{product.Name}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Update(Product product, CancellationToken cancellation)
    {
        try
        {
            _products.RemoveAll(p => p.Id == product.Id);
            _products.Add(product);
            return await Task.FromResult(ValidationResult.Success!);
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Product '{product.Name}': {ex.Message}");
        }
    }
}
