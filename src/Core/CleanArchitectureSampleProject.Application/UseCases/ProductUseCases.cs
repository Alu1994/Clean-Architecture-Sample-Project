using System.Collections.Frozen;
using CleanArchitectureSampleProject.Application.Inputs;
using CleanArchitectureSampleProject.Application.Outputs;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Domain.AggregateRoots.Products.Services;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Application.UseCases;

public interface IProductUseCases
{
    Task<Validation<Error, FrozenSet<GetProductOutput>>> GetProducts(CancellationToken cancellation);
    Task<Validation<Error, GetProductOutput>> GetProductById(Guid productId, CancellationToken cancellation);
    Task<Validation<Error, GetProductOutput>> GetProductByName(string productName, CancellationToken cancellation);
    Task<Validation<Error, CreateProductOutput>> CreateProduct(CreateProductInput productInput, CancellationToken cancellation);
    Task<Validation<Error, UpdateProductOutput>> UpdateProduct(UpdateProductInput productInput, CancellationToken cancellation);
}

public sealed class ProductUseCases(ILogger<ProductUseCases> logger, IProductRepository productRepository, ICategoryUseCases categoryUseCases, ICreateProductService createProductService) : IProductUseCases
{
    private readonly ILogger<ProductUseCases> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly ICategoryUseCases _categoryUseCases = categoryUseCases ?? throw new ArgumentNullException(nameof(categoryUseCases));
    private readonly ICreateProductService _createProductService = createProductService ?? throw new ArgumentNullException(nameof(createProductService));

    public async Task<Validation<Error, FrozenSet<GetProductOutput>>> GetProducts(CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName}", nameof(GetProducts));
        var result = await _productRepository.Get(cancellation);
        return result.Match<Validation<Error, FrozenSet<GetProductOutput>>>(
            products =>
                products.Select<Product, GetProductOutput>(product => product).ToFrozenSet(),
            error => error);
    }

    public async Task<Validation<Error, GetProductOutput>> GetProductById(Guid productId, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductId}", nameof(GetProductById), productId);
        var result = await _productRepository.GetById(productId, cancellation);
        return result.Match<Validation<Error, GetProductOutput>>(
            product => (GetProductOutput)product,
            error => error);
    }

    public async Task<Validation<Error, GetProductOutput>> GetProductByName(string productName, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductName}", nameof(GetProductByName), productName);
        var result = await _productRepository.GetByName(productName, cancellation);
        return result.Match<Validation<Error, GetProductOutput>>(
            product => (GetProductOutput)product,
            error => error);
    }

    public async Task<Validation<Error, CreateProductOutput>> CreateProduct(CreateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(CreateProduct), productInput);

        var product = productInput.ToProduct();
        Category category = productInput.Category.ToCategory2();
        var result = await _createProductService.Execute(product, category, cancellation);

        if(result.IsFail) return result.ToError();

        return (CreateProductOutput)result.ToSuccess()!;
    }

    public async Task<Validation<Error, UpdateProductOutput>> UpdateProduct(UpdateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(UpdateProduct), productInput);
        
        var getCategoryResult = await _categoryUseCases.GetOrCreateCategoryInternal(productInput.Category, cancellation);        
        if (getCategoryResult.IsFail) return getCategoryResult.ToError();

        var categoryResult = getCategoryResult.ToSuccess().ToCategory()!;
        if (categoryResult.IsFail) return categoryResult.ToError();

        var category = categoryResult.ToSuccess();
        var getProductByIdResult = await GetProductById(productInput.Id, cancellation);
        if (getProductByIdResult.IsFail) return getProductByIdResult.ToError();

        var productOutputResult = getProductByIdResult.ToSuccess();
        productInput.SetCategory(category);
        var productResult = productInput.ToProduct(productOutputResult.CreationDate);
        if (productResult.IsFail) return productResult.ToError();
        
        var product = productResult.ToSuccess();
        product.SetCategory(category);
        var repoResult = await _productRepository.Update(product, cancellation);
        if (repoResult != ValidationResult.Success) return Error.New(repoResult.ErrorMessage!);

        product.WithCategory(category);
        return (UpdateProductOutput)product;
    }
}
