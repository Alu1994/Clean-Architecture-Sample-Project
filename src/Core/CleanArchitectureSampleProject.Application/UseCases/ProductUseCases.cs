using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Domain.AggregateRoots.Products.Services;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Application.UseCases;

public interface IProductUseCases
{
    Task<Results<FrozenSet<GetProductOutput>, BaseError>> GetProducts(CancellationToken cancellation);
    Task<Results<GetProductOutput, BaseError>> GetProductById(Guid productId, CancellationToken cancellation);
    Task<Results<GetProductOutput, BaseError>> GetProductByName(string productName, CancellationToken cancellation);
    Task<Results<CreateProductOutput, BaseError>> CreateProduct(CreateProductInput productInput, CancellationToken cancellation);
    Task<Results<UpdateProductOutput, BaseError>> UpdateProduct(UpdateProductInput productInput, CancellationToken cancellation);
}

public sealed class ProductUseCases(
    ILogger<ProductUseCases> logger, 
    IProductRepository productRepository, 
    ICreateProductService createProductService, 
    IUpdateProductService updateProductService) : IProductUseCases
{
    private readonly ILogger<ProductUseCases> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly ICreateProductService _createProductService = createProductService ?? throw new ArgumentNullException(nameof(createProductService));
    private readonly IUpdateProductService _updateProductService = updateProductService ?? throw new ArgumentNullException(nameof(updateProductService));

    public async Task<Results<FrozenSet<GetProductOutput>, BaseError>> GetProducts(CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName}", nameof(GetProducts));

        var result = await _productRepository.Get(cancellation);
        return result.Match<Results<FrozenSet<GetProductOutput>, BaseError>>(
            products =>
                products.Select<Product, GetProductOutput>(product => product).ToFrozenSet(),
            error => error);
    }

    public async Task<Results<GetProductOutput, BaseError>> GetProductById(Guid productId, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductId}", nameof(GetProductById), productId);

        var result = await _productRepository.GetById(productId, cancellation);
        return result.Match<Results<GetProductOutput, BaseError>>(
            product => (GetProductOutput)product,
            error => error);
    }

    public async Task<Results<GetProductOutput, BaseError>> GetProductByName(string productName, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductName}", nameof(GetProductByName), productName);

        var result = await _productRepository.GetByName(productName, cancellation);
        if (result.IsSuccess)
            return (GetProductOutput)result.Success!;
        return result.Error!;
    }

    public async Task<Results<CreateProductOutput, BaseError>> CreateProduct(CreateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(CreateProduct), productInput);

        Category category = productInput.Category.ToCategory();
        var product = productInput.ToProduct(category);
        var result = await _createProductService.Execute(product, cancellation);
        if(result.IsFail) return result.ToError();
        return (CreateProductOutput)result.ToSuccess()!;
    }

    public async Task<Results<UpdateProductOutput, BaseError>> UpdateProduct(UpdateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(UpdateProduct), productInput);

        Category category = productInput.Category.ToCategory();
        var product = productInput.ToProduct(category);
        var result = await _updateProductService.Execute(product, cancellation);
        if (result.IsFail) return result.ToError();
        return (UpdateProductOutput)result.ToSuccess()!;
    }
}
