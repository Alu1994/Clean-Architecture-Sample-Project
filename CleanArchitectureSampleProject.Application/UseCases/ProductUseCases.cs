﻿using System.Collections.Frozen;
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

        var categoryResult = await _categoryUseCases.GetOrCreateCategory(productInput, cancellation);
        return await categoryResult.MatchAsync(async category =>
            await CreateProduct(productInput, category, cancellation),
            error => error);

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

        var categoryResult = await _categoryUseCases.GetOrCreateCategory(new CreateProductInput(productInput), cancellation);
        return await categoryResult.MatchAsync(async category =>
            await UpdateProduct(productInput, category, cancellation),
            error => error);

        Task<Validation<Error, UpdateProductOutput>> UpdateProduct(UpdateProductInput productInput, Category category, CancellationToken cancellation)
        {
            productInput.SetCategory(category);
            var result = productInput.ToProduct();
            return result.MatchAsync<Validation<Error, UpdateProductOutput>>(async product =>
            {
                product.SetCategory(category);
                var repoResult = await _productRepository.Update(product, cancellation);
                if (repoResult != ValidationResult.Success)
                    return Error.New(repoResult.ErrorMessage!);
                product.WithCategory(category);
                return (UpdateProductOutput)product;
            }, error => error);
        }
    }
}