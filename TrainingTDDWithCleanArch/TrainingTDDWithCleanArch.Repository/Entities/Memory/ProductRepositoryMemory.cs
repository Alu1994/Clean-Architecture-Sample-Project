using CleanArchitectureSampleProject.CrossCuttingConcerns;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products;
using TrainingTDDWithCleanArch.Domain.Interfaces;

namespace TrainingTDDWithCleanArch.Repository.Entities.Memory;

public sealed class ProductRepositoryMemory(ILogger<ProductRepositoryMemory> logger, ICategoryRepository categoryRepository) : IProductRepository
{
    private readonly ILogger<ProductRepositoryMemory> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));

    //private List<Product> _products = [ Product.Create("Product", "Product", 0, 0, Domain.AggregateRoots.Products.Entities.Category.Create(Guid.NewGuid(), "Category").SuccessToSeq().First()).SuccessToSeq().First() ];
    private List<Product> _products = [];

    public async Task<Validation<Error, FrozenSet<Product>>> Get(CancellationToken cancellation)
    {
        try
        {
            var products = await Task.FromResult(_products.ToFrozenSet());
            foreach (var product in products)
            {
                var categoryResult = await _categoryRepository.GetById(product.Category.Id, cancellation);
                _ = categoryResult.Match<Validation<Error, Product>>(category =>
                {
                    return product.WithCategory(category);
                }, erCat =>
                {
                    _logger.LogSeqError(erCat);
                    return erCat;
                });
            }
            return products;
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving all Products: {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Product>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            return await Task.FromResult(_products.First(x => x.Id == id));
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Product with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Product>> GetByName(string productName, CancellationToken cancellation)
    {
        try
        {
            return await Task.FromResult(_products.First(x => x.Name == productName));
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Product with name '{productName}': {ex.Message}", ex);
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
