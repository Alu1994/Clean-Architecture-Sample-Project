using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products;
using TrainingTDDWithCleanArch.Domain.Interfaces;

namespace TrainingTDDWithCleanArch.Repository.Entities.Memory;

public sealed class ProductRepository(ILogger<ProductRepository> logger) : IProductRepository
{
    private readonly ILogger<ProductRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    //private List<Product> _products = [ Product.Create("Product", "Product", 0, 0, Domain.AggregateRoots.Products.Entities.Category.Create(Guid.NewGuid(), "Category").SuccessToSeq().First()).SuccessToSeq().First() ];
    private List<Product> _products = [];

    public async Task<Validation<Error, FrozenSet<Product>>> Get(CancellationToken cancellation)
    {
        try
        {
            return await Task.FromResult(_products.ToFrozenSet());
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving all Products: {ex.Message}");
        }
    }

    public async Task<Validation<Error, Product>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            _logger.LogInformation("Logging {MethodName} with {Id}", nameof(GetById), id);
            return await Task.FromResult(_products.First(x => x.Id == id));
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Product with id '{id}': {ex.Message}");
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
            return Error.New($"Error while retrieving Product with name '{productName}': {ex.Message}");
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
        throw new NotImplementedException();
    }
}
