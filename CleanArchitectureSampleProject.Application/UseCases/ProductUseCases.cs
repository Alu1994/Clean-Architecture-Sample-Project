using System.Collections.Frozen;
using CleanArchitectureSampleProject.Application.Inputs;
using CleanArchitectureSampleProject.Application.Outputs;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Repositories;

namespace CleanArchitectureSampleProject.Application.UseCases;

public interface IProductUseCases
{
    Task<Validation<Error, FrozenSet<GetProductOutput>>> GetProducts(CancellationToken cancellation);
    Task<Validation<Error, GetProductOutput>> GetProductById(Guid productId, CancellationToken cancellation);
    Task<Validation<Error, GetProductOutput>> GetProductByName(string productName, CancellationToken cancellation);
    Task<Validation<Error, CreateProductOutput>> CreateProduct(CreateProductInput productInput, CancellationToken cancellation);
    Task<Validation<Error, UpdateProductOutput>> UpdateProduct(UpdateProductInput productInput, CancellationToken cancellation);
}

public sealed class ProductUseCases(ILogger<ProductUseCases> logger, IProductRepository productRepository, ICategoryUseCases categoryUseCases) : IProductUseCases
{
    private readonly ILogger<ProductUseCases> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly ICategoryUseCases _categoryUseCases = categoryUseCases ?? throw new ArgumentNullException(nameof(categoryUseCases));

    public async Task<Validation<Error, FrozenSet<GetProductOutput>>> GetProducts(CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName}", nameof(GetProducts));
        var result = await _productRepository.Get(cancellation);
        return result.Match<Validation<Error, FrozenSet<GetProductOutput>>>(products =>
        {
            var r = products.Select<Product, GetProductOutput>(x => { return x; }).ToFrozenSet();
            return r;
        }, error => error);
    }

    public async Task<Validation<Error, GetProductOutput>> GetProductById(Guid productId, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductId}", nameof(GetProductById), productId);
        var result = await _productRepository.GetById(productId, cancellation);
        return result.Match<Validation<Error, GetProductOutput>>(product =>
        {
            return (GetProductOutput)product;
        }, error => error);
    }

    public async Task<Validation<Error, GetProductOutput>> GetProductByName(string productName, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductName}", nameof(GetProductByName), productName);
        var result = await _productRepository.GetByName(productName, cancellation);
        return result.Match<Validation<Error, GetProductOutput>>(product =>
        {
            return (GetProductOutput)product;
        }, error => error);
    }

    public async Task<Validation<Error, CreateProductOutput>> CreateProduct(CreateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(CreateProduct), productInput);

        var getOrCreateCategoryResult = await _categoryUseCases.GetOrCreateCategory(productInput, cancellation);
        return await getOrCreateCategoryResult.MatchAsync(async getOrCreateCategory =>
        {
            var category = getOrCreateCategory.ToCategory();
            return await category.MatchAsync(
                async cat => await CreateProduct(productInput, cat, cancellation)
                , er => er);
        }, error => error);

        Task<Validation<Error, CreateProductOutput>> CreateProduct(CreateProductInput productInput, Category category, CancellationToken cancellation)
        {
            return productInput.ToProduct().MatchAsync<Validation<Error, CreateProductOutput>>(async product =>
            {
                product.SetCategory(category);
                var repoResult = await _productRepository.Insert(product, cancellation);
                if (repoResult != ValidationResult.Success)
                    return Error.New(repoResult.ErrorMessage!);
                product.WithCategory(category);
                return (CreateProductOutput)product;
            }, error => error);
        }
    }

    public async Task<Validation<Error, UpdateProductOutput>> UpdateProduct(UpdateProductInput productInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {ProductInput}", nameof(UpdateProduct), productInput);
        var getResult = await _categoryUseCases.GetOrCreateCategoryInternal(productInput.Category, cancellation);
        return await getResult.MatchAsync(async getOrCreateCategory => await ConvertCategoryAndUpdateProduct(productInput, getOrCreateCategory, cancellation), e => e);

        async Task<Validation<Error, UpdateProductOutput>> ConvertCategoryAndUpdateProduct(UpdateProductInput productInput, CategoryOutput existentCategoryOutput, CancellationToken cancellation)
        {
            var categoryResult = existentCategoryOutput.ToCategory();
            return await categoryResult.MatchAsync(async category => await UpdateProductWithCategory(productInput, category, cancellation), toCategoryError => toCategoryError);
        }
    }

    private async Task<Validation<Error, UpdateProductOutput>> UpdateProductWithCategory(UpdateProductInput productInput, Category category, CancellationToken cancellation)
    {
        var getProductByIdResult = await GetProductById(productInput.Id, cancellation);
        return await getProductByIdResult.MatchAsync(async getProductById => await ConvertAndUpdateProduct(productInput, category, getProductById, cancellation), error => error);

        async Task<Validation<Error, UpdateProductOutput>> ConvertAndUpdateProduct(UpdateProductInput productInput, Category category, GetProductOutput getProductById, CancellationToken cancellation)
        {
            productInput.SetCategory(category);
            var productResult = productInput.ToProduct(getProductById.CreationDate);
            return await UpdateProduct(category, productResult, cancellation);
        }
    }

    private async Task<Validation<Error, UpdateProductOutput>> UpdateProduct(Category category, Validation<Error, Product> productResult, CancellationToken cancellation)
    {
        return await productResult.MatchAsync(async product => await Update(category, product, cancellation), er => er);

        async Task<Validation<Error, UpdateProductOutput>> Update(Category category, Product product, CancellationToken cancellation)
        {
            product.SetCategory(category);
            var repoResult = await _productRepository.Update(product, cancellation);
            if (repoResult != ValidationResult.Success)
                return Error.New(repoResult.ErrorMessage!);
            product.WithCategory(category);
            return (UpdateProductOutput)product;
        }
    }
}
